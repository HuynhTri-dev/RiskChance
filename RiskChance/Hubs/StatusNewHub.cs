using Microsoft.AspNetCore.SignalR;

namespace RiskChance.Hubs
{
    public class StatusNewHub : Hub
    {
        public async Task UpdateStatus(int id, int newStatus)
        {
            await Clients.All.SendAsync("ReceiveStatusNews", id, newStatus);
        }
    }
}
