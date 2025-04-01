using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepository<ThongBao> _notifRepo;
        private readonly ApplicationDBContext _context;

        public NotificationController(IHubContext<NotificationHub> hubContext, IRepository<ThongBao> notifRepo, ApplicationDBContext context)
        {
            _hubContext = hubContext;
            _notifRepo = notifRepo;
            _context = context;
        }

        public async Task<IActionResult> NotifyList()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var notifyList = await _context.ThongBaos
                .Include(x => x.NguoiGui)
                .Where(x => x.IDNguoiNhan == userId && x.TrangThai == TrangThaiThongBao.ChuaDoc)
                .OrderBy(x => x.NgayGui)
                .ToListAsync();

            return View("_NotifyPartial", notifyList);
        }

        // Đánh dấu thông báo là đã đọc
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _context.ThongBaos.FindAsync(notificationId);
            if (notification != null)
            {
                notification.TrangThai = TrangThaiThongBao.DaDoc;
                _context.Update(notification);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
