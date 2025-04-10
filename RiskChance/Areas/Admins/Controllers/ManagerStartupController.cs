using Microsoft.AspNetCore.Authorization;
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
    public class ManagerStartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<StatusStartupHub> _hubContext;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<Startup> _startupRepo;
        private readonly IRepository<DanhGiaStartup> _danhGiaStartupRepo;
        private readonly NotificationService _notificationService;

        public ManagerStartupController(ApplicationDBContext context, 
                                        IHubContext<StatusStartupHub> hubContext,
                                        UserManager<NguoiDung> userManager,
                                        IRepository<Startup> startupRepo,
                                        IRepository<DanhGiaStartup> danhGiaStartupRepo,
                                        NotificationService notificationService)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
            _startupRepo = startupRepo;
            _danhGiaStartupRepo = danhGiaStartupRepo;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "manageStartup";
            var startups = await _context.Startups.Include(x => x.LinhVuc).ToListAsync();
            return View(startups);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var startup = await _startupRepo.GetByIdAsync(id);
            if (startup == null)
            {
                return Json(new { success = false });
            }

            var idAdmin = HttpContext.Session.GetString("UserId");

            var admin = await _userManager.GetUserAsync(User);

            if (admin == null)
            {
                return Json(new { success = false });
            }

            // Đổi trạng thái vòng tròn: Chờ duyệt -> Đã duyệt -> Từ chối -> Chờ duyệt
            startup.TrangThaiXetDuyet = (TrangThaiXetDuyetEnum)(((int)startup.TrangThaiXetDuyet + 1) % 3);

            if (startup.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
            {
                var danhGiaMacDinh = new DanhGiaStartup
                {
                    DiemDanhGia = 5,
                    NhanXet = "",
                    NgayDanhGia = DateTime.Now,
                    IDStartup = startup.IDStartup,
                    IDNguoiDung = admin.Id
                };

                await _danhGiaStartupRepo.AddAsync(danhGiaMacDinh);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout startup has been accepted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = startup.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);

            }
            else
            {
                var danhGias = _context.DanhGiaStartups.Where(d => d.IDStartup == id);
                _context.DanhGiaStartups.RemoveRange(danhGias);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout startup has been unaccepted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = startup.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);

                await _context.SaveChangesAsync();
            }

            // Gửi sự kiện qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", id, (int)startup.TrangThaiXetDuyet);

            return Json(new { success = true, newStatus = (int)startup.TrangThaiXetDuyet });
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateRequest request)
        {
            if (request == null || request.Ids == null || !request.Ids.Any())
            {
                return Json(new { success = false });
            }

            var admin = await _userManager.GetUserAsync(User);

            if (admin == null)
            {
                return Json(new { success = false });
            }

            foreach (var id in request.Ids)
            {
                var startup = await _startupRepo.GetByIdAsync(id);
                if (startup != null)
                {
                    startup.TrangThaiXetDuyet = (TrangThaiXetDuyetEnum)(((int)startup.TrangThaiXetDuyet + 1) % 3);

                    if (startup.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                    {
                        var danhGiaMacDinh = new DanhGiaStartup
                        {
                            DiemDanhGia = 5,
                            NhanXet = "",
                            NgayDanhGia = DateTime.Now,
                            IDStartup = startup.IDStartup,
                            IDNguoiDung = admin.Id
                        };

                        await _danhGiaStartupRepo.AddAsync(danhGiaMacDinh);

                        var notif = new ThongBao
                        {
                            IDNguoiGui = HttpContext.Session.GetString("UserId"),
                            NoiDung = $"Yout startup has been accepted",
                            NgayGui = DateTime.Now,
                            IDNguoiNhan = startup.IDNguoiDung,
                        };

                        await _notificationService.SendNotification(notif);
                    }
                    else
                    {
                        var danhGias = _context.DanhGiaStartups.Where(d => d.IDStartup == id);
                        _context.DanhGiaStartups.RemoveRange(danhGias);

                        var notif = new ThongBao
                        {
                            IDNguoiGui = HttpContext.Session.GetString("UserId"),
                            NoiDung = $"Yout startup has been unaccepted",
                            NgayGui = DateTime.Now,
                            IDNguoiNhan = startup.IDNguoiDung,
                        };

                        await _notificationService.SendNotification(notif);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                // Gửi thông báo đến tất cả client bằng SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", 1, 1);

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


        [HttpPost]
        public async Task<IActionResult> DeleteStartup(int id)
        {
            try
            {
                var startup = await _context.Startups
                .Include(s => s.GiayTos)
                .Include(s => s.DanhGiaStartups)
                .FirstOrDefaultAsync(s => s.IDStartup == id);

                if (startup == null)
                    return Json(new { success = false, message = "Startup is not valid!" });


                _context.GiayTos.RemoveRange(startup.GiayTos);
                _context.DanhGiaStartups.RemoveRange(startup.DanhGiaStartups);
                await _startupRepo.DeleteAsync(id);

                var notif = new ThongBao
                {
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    NoiDung = $"Yout startup has been deleted",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = startup.IDNguoiDung,
                };

                await _notificationService.SendNotification(notif);

                // Gửi sự kiện qua SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", id, (int)startup.TrangThaiXetDuyet);

                return Json(new { success = true, newStatus = (int)startup.TrangThaiXetDuyet });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error when delete startup: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchStartup(string keyword)
        {
            var startsups = await _context.Startups
                                .Where(x => string.IsNullOrEmpty(keyword) || x.TenStartup.ToLower().Contains(keyword.ToLower()))
                                .ToListAsync();

            return View("Index", startsups);
        }
    }
}
