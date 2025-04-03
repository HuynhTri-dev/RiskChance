using Microsoft.AspNetCore.SignalR;
using RiskChance.Models;

namespace RiskChance.Hubs
{
    public class PostCommentNewsHub : Hub
    {
        public async Task SendComment(BinhLuanTinTuc comment)
        {
            await Clients.All.SendAsync("ReceiveCommentNews", comment);
        }
    }
}
