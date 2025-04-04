using Microsoft.AspNetCore.SignalR;
using RiskChance.Models;

namespace RiskChance.Hubs
{
    public class MessengerHub : Hub
    {
        public async Task SenMess(string idReceiver, TinNhan message)
        {
            await Clients.User(idReceiver).SendAsync("ReceiveMess", message);
        }
    }
}
