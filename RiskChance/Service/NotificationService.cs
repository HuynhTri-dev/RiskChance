using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IRepository<ThongBao> _notifRepo;
    private readonly ApplicationDBContext _context;

    public NotificationService(IHubContext<NotificationHub> hubContext, IRepository<ThongBao> notifRepo, ApplicationDBContext context)
    {
        _hubContext = hubContext;
        _notifRepo = notifRepo;
        _context = context;
    }

    public async Task SendNotification(ThongBao notif)
    {
        // Lưu thông báo vào database
        await _notifRepo.AddAsync(notif);

        // Gửi thông báo qua SignalR
        await _hubContext.Clients.User(notif.IDNguoiNhan).SendAsync("ReceiveNotification", notif);
    }
}
