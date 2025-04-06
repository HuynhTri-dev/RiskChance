using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CommentNewsController : Controller
    {
        private readonly IRepository<BinhLuanTinTuc> _commentNewsRepo;
        private readonly IHubContext<PostCommentNewsHub> _hubContext;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CommentNewsController> _logger;


        public CommentNewsController(IRepository<BinhLuanTinTuc> commentNewsRepo, 
                                    IHubContext<PostCommentNewsHub> hubContext, 
                                    ApplicationDBContext context,
                                    ILogger<CommentNewsController> logger)
        {
            _commentNewsRepo = commentNewsRepo;
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        // POST: CommentController/Create
        [HttpPost]
        public async Task<IActionResult> Create(BinhLuanTinTuc model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState không hợp lệ");
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để bình luận.";
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            var tinTuc = await _context.TinTucs.FindAsync(model.IDTinTuc);
            if (tinTuc == null)
            {
                _logger.LogWarning($"Tin tức không tìm thấy với ID: {model.IDTinTuc}");
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            model.IDNguoiDung = userId;
            model.NgayBinhLuan = DateTime.Now;

            await _commentNewsRepo.AddAsync(model);

            var commentNews = await _context.BinhLuanTinTucs.Include(x => x.NguoiDung)
                .FirstOrDefaultAsync(x => x.IDBinhLuan == model.IDBinhLuan);

            if (commentNews == null)
            {
                _logger.LogError("Không thể tải dữ liệu comment vừa thêm.");
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            try
            {
                await _hubContext.Clients.Group(model.IDTinTuc.ToString()).SendAsync("ReceiveCommentNews", commentNews);
                _logger.LogInformation("Đã gửi bình luận thành công đến nhóm.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi gửi bình luận qua SignalR: {ex.Message}");
            }

            return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return Json("Message");
            var commentNews = await _commentNewsRepo.GetByIdAsync(id);
            if (commentNews == null) return NotFound();
            return PartialView("_EditCommentNewsPartial", commentNews);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BinhLuanTinTuc model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            var tinTuc = await _context.TinTucs.FindAsync(model.IDTinTuc);

            if (tinTuc == null)
            {
                // Log lỗi
                Console.WriteLine($"Lỗi: Không tìm thấy IDTinTuc = {model.IDTinTuc}");
                return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
            }

            model.NgayBinhLuan = DateTime.Now;

            await _commentNewsRepo.UpdateAsync(model);

            var commentNews = await _context.BinhLuanTinTucs.Include(x => x.NguoiDung)
                                    .FirstOrDefaultAsync(x => x.IDBinhLuan == model.IDBinhLuan);

            await _hubContext.Clients.All.SendAsync("ReceiveCommentNews", model);

            return RedirectToAction("Details", "News", new { area = "", id = model.IDTinTuc });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commentNews = await _commentNewsRepo.GetByIdAsync(id);

            await _commentNewsRepo.DeleteAsync(id);

            return RedirectToAction("Details", "News", new { area = "", id = commentNews.IDTinTuc });
        }
    }
}
