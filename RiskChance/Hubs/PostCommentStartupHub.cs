using Microsoft.AspNetCore.SignalR;

namespace RiskChance.Hubs
{
    public class PostCommentStartupHub : Hub
    {
        public async Task GetComment(int id, string content, double diem)
        {
            await Clients.All.SendAsync("ReceiveComment", id, content, diem);
        }
    }
}
