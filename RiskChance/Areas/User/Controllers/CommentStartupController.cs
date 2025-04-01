using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Repositories;
using System.Security.Claims;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CommentStartupController : Controller
    {
        private readonly IRepository<DanhGiaStartup> _commentRepo;
        private readonly IHubContext<StatusStartupHub> _hubContext;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly ApplicationDBContext _context;

        public CommentStartupController(IRepository<DanhGiaStartup> commentRepo, 
            IHubContext<StatusStartupHub> hubContext, 
            UserManager<NguoiDung> userManager,
             ApplicationDBContext context)
        {
            _commentRepo = commentRepo;
            _hubContext = hubContext;
            _userManager = userManager;
            _context = context;
        }

        // POST: CommentController/Create
        [HttpPost]
        public async Task<IActionResult> Create(DanhGiaStartup model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
            }

            var userId = HttpContext.Session.GetString("UserId");

            model.IDNguoiDung = userId;
            model.NgayDanhGia = DateTime.Now;

            await _commentRepo.AddAsync(model);

            await _hubContext.Clients.All.SendAsync("ReceiveComment", model.IDNguoiDung, model.NhanXet, model.DiemDanhGia, model.NguoiDung?.AvatarUrl, model.NgayDanhGia.ToString("dd/MM/yyyy HH:mm:ss"));

            return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
        }

        // GET: CommentController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return Json("Message");
            var commentStartup = await _commentRepo.GetByIdAsync(id);
            if (commentStartup == null) return NotFound();
            return PartialView("_EditCommentStartupPartial", commentStartup);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DanhGiaStartup model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để chỉnh sửa đánh giá.";
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
            }

            // Kiểm tra xem đánh giá có tồn tại không
            var existingComment = await _context.DanhGiaStartups.FindAsync(model.IDDanhGia);
            if (existingComment == null)
            {
                TempData["ErrorMessage"] = "Đánh giá không tồn tại.";
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
            }

            // Cập nhật thông tin đánh giá
            existingComment.DiemDanhGia = model.DiemDanhGia;
            existingComment.NhanXet = model.NhanXet;
            existingComment.NgayDanhGia = DateTime.Now;

            await _commentRepo.UpdateAsync(existingComment);

            var commentNews = await _context.DanhGiaStartups.Include(x => x.NguoiDung)
                .FirstOrDefaultAsync(x => x.IDDanhGia == model.IDDanhGia);

            // Gửi dữ liệu đến client bằng SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveComment", commentNews);

            TempData["SuccessMessage"] = "Đánh giá đã được cập nhật.";
            return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commentStartup = await _commentRepo.GetByIdAsync(id);

            await _commentRepo.DeleteAsync(id);

            return RedirectToAction("Details", "Startup", new { area = "", id = commentStartup.IDStartup });
        }
    }
}
