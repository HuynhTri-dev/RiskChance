using Microsoft.AspNetCore.SignalR;
using RiskChance.Models;

namespace RiskChance.Hubs
{
    public class PostCommentStartupHub : Hub
    {
        public  async Task SendComment(string IDNguoiDang, string Comment, float DiemDanhGia, string AvatarUrl, DateTime NgayDanhGia)
        {
            await Clients.All.SendAsync("ReceiveComment", IDNguoiDang, Comment, DiemDanhGia, AvatarUrl, NgayDanhGia);
        }
    }
}
    