using Microsoft.AspNetCore.SignalR;
using RiskChance.Models;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    public async Task SendNotification(string userId, ThongBao message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }
}