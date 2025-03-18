using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    [Authorize(Roles = "Admin")]
    public class ManagerStartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<StatusStartupHub> _hubContext;

        public ManagerStartupController(ApplicationDBContext context, IHubContext<StatusStartupHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "manageStartup";
            var startups = await _context.Startups.ToListAsync();
            return View(startups);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var startup = _context.Startups.Find(id);
            if (startup == null)
                return Json(new { success = false });

            // Đổi trạng thái vòng tròn: Chờ duyệt -> Đã duyệt -> Từ chối -> Chờ duyệt
            startup.TrangThaiXetDuyet = (TrangThaiXetDuyetEnum)(((int)startup.TrangThaiXetDuyet + 1) % 3);
            _context.SaveChanges();

            // Gửi sự kiện qua SignalR
            _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", id, (int)startup.TrangThaiXetDuyet);

            return Json(new { success = true, newStatus = (int)startup.TrangThaiXetDuyet });
        }

        [HttpPost]
        public IActionResult DeleteStartup(int id)
        {
            var startup = _context.Startups.Find(id);
            if (startup == null)
                return Json(new { success = false });

            _context.Startups.Remove(startup);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
