using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public CommentStartupController(IRepository<DanhGiaStartup> commentRepo, IHubContext<StatusStartupHub> hubContext, UserManager<NguoiDung> userManager)
        {
            _commentRepo = commentRepo;
            _hubContext = hubContext;
            _userManager = userManager;
        }
        // POST: CommentController/Create
        [HttpPost]
        public async Task<IActionResult> Create(DanhGiaStartup model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup, error = "User not found" });
            }

            model.IDNguoiDung = user.Id;
            model.NgayDanhGia = DateTime.Now;

            await _commentRepo.AddAsync(model);

            await _hubContext.Clients.All.SendAsync("ReceiveComment", model.IDNguoiDung, model.NhanXet, model.DiemDanhGia, model.NguoiDung?.AvatarUrl, model.NgayDanhGia.ToString("dd/MM/yyyy HH:mm:ss"));

            return RedirectToAction("Details", "Startup", new { area = "", id = model.IDStartup });
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();  
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
