using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RiskChance.Hubs
{
    public class StatusStartupHub : Hub
    {
        public async Task UpdateStatus(int id, int newStatus)
        {
            await Clients.All.SendAsync("ReceiveStatusUpdate", id, newStatus);
        }

        public async Task AddNewStartup(string message)
        {
            await Clients.All.SendAsync("ReceiveStartupAdd", message);
        }
    }
}
