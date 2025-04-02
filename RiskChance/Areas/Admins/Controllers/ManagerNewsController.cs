using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    [Authorize(Roles = "Admin")]
    public class ManagerNewsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<StatusNewHub> _hubContext;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<TinTuc> _newsRepo;
        private readonly IRepository<BinhLuanTinTuc> _commentNewsRepo;
        private readonly NotificationService _notificationService;

        public ManagerNewsController(ApplicationDBContext context,
                                        IHubContext<StatusNewHub> hubContext,
                                        UserManager<NguoiDung> userManager,
                                        IRepository<TinTuc> newsRepo,
                                        IRepository<BinhLuanTinTuc> commentNewsRepo,
                                        NotificationService notificationService)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _newsRepo = newsRepo;
            _commentNewsRepo = commentNewsRepo;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            ViewBag.ActiveFeature = "manageNews";
            var newsQuery = _context.TinTucs.Include(x => x.NguoiDung).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                newsQuery = newsQuery.Where(n => n.TieuDe.Contains(search));
            }

            var newsList = await newsQuery.ToListAsync();
            return View(newsList);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false });
            }

            var news = await _newsRepo.GetByIdAsync(id);

            if (news == null)
            {
                return Json(new { success = false });
            }
            news.NgayDang = DateTime.Now;
            // Đổi trạng thái tin tức: Chờ duyệt -> Đã duyệt -> Từ chối -> Chờ duyệt
            news.TrangThaiXetDuyet = (TrangThaiXetDuyetEnum)(((int)news.TrangThaiXetDuyet + 1) % 3);
           

            if (news.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
            {
                var danhGiaMacDinh = new BinhLuanTinTuc
                {
                    DiemDanhGia = 5,
                    NhanXet = "",
                    NgayBinhLuan = DateTime.Now,
                    IDTinTuc = news.IDTinTuc,
                    IDNguoiDung = HttpContext.Session.GetString("UserId")
                };

                await _commentNewsRepo.AddAsync(danhGiaMacDinh);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout news has been accepted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = news.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);
            }
            else
            {
                
                var danhGias = await _context.BinhLuanTinTucs.Where(d => d.IDTinTuc == id).ToListAsync();
                _context.BinhLuanTinTucs.RemoveRange(danhGias);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout news has been unaccepted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = news.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);

                await _context.SaveChangesAsync();
            }

            // Gửi sự kiện qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveStatusNews", id, (int)news.TrangThaiXetDuyet);

            return Json(new { success = true, newStatus = (int)news.TrangThaiXetDuyet });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var news = _context.TinTucs
                    .Include(x => x.TinTucHashtags)
                    .Include(n => n.BinhLuanTinTucs)
                    .FirstOrDefault(n => n.IDTinTuc == id);

                if (news == null)
                    return Json(new { success = false, message = "This new is not available!" });

                _context.TinTucHashtags.RemoveRange(news.TinTucHashtags);
                _context.BinhLuanTinTucs.RemoveRange(news.BinhLuanTinTucs);

                await _newsRepo.DeleteAsync(id);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout news has been deleted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = news.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);

                // Gửi sự kiện qua SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveStatusNews", id, (int)news.TrangThaiXetDuyet);

                return Json(new { success = true, newStatus = (int)news.TrangThaiXetDuyet });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Delete Unsuccess!" + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateRequest request)
        {
            if (request == null || request.Ids == null || !request.Ids.Any())
            {
                return Json(new { success = false });
            }

            var admin = HttpContext.Session.GetString("UserId");

            if (admin == null)
            {
                return Json(new { success = false });
            }

            foreach (var id in request.Ids)
            {
                var news = await _newsRepo.GetByIdAsync(id);
                if (news != null)
                {
                    news.TrangThaiXetDuyet = (TrangThaiXetDuyetEnum)(((int)news.TrangThaiXetDuyet + 1) % 3);


                    if (news.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                    {
                        var danhGiaMacDinh = new BinhLuanTinTuc
                        {
                            DiemDanhGia = 5,
                            NhanXet = "",
                            NgayBinhLuan = DateTime.Now,
                            IDTinTuc = news.IDTinTuc,
                            IDNguoiDung = HttpContext.Session.GetString("UserId")
                        };

                        var notif = new ThongBao
                        {
                            IDNguoiGui = HttpContext.Session.GetString("UserId"),
                            NoiDung = $"Yout news has been accepted",
                            NgayGui = DateTime.Now,
                            IDNguoiNhan = news.IDNguoiDung,
                        };

                        await _notificationService.SendNotification(notif);

                        await _commentNewsRepo.AddAsync(danhGiaMacDinh);
                    }
                    else
                    {
                        var notif = new ThongBao
                        {
                            IDNguoiGui = HttpContext.Session.GetString("UserId"),
                            NoiDung = $"Yout news has been unaccepted",
                            NgayGui = DateTime.Now,
                            IDNguoiNhan = news.IDNguoiDung,
                        };

                        await _notificationService.SendNotification(notif);

                        var danhGias = await _context.BinhLuanTinTucs.Where(d => d.IDTinTuc == id).ToListAsync();
                        _context.BinhLuanTinTucs.RemoveRange(danhGias);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                // Gửi thông báo đến tất cả client bằng SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveStatusNews", 1, 1);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
            }
        }

        public class BulkUpdateRequest
        {
            public List<int> Ids { get; set; }
        }
    }
}
