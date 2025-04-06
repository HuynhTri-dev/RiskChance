using Microsoft.AspNetCore.SignalR;
using RiskChance.Models;

namespace RiskChance.Hubs
{
    public class PostCommentNewsHub : Hub
    {
        public async Task JoinGroup(string newsId)
        {
            if (string.IsNullOrEmpty(newsId))
            {
                Console.WriteLine("newsId không hợp lệ.");
                return;
            }

            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, newsId);
                Console.WriteLine($"Kết nối {Context.ConnectionId} đã tham gia nhóm {newsId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tham gia nhóm {newsId}: {ex.Message}");
            }
        }

        public async Task LeaveGroup(string newsId)
        {
            if (string.IsNullOrEmpty(newsId))
            {
                Console.WriteLine("newsId không hợp lệ.");
                return;
            }

            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, newsId);
                Console.WriteLine($"Kết nối {Context.ConnectionId} đã rời nhóm {newsId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi rời nhóm {newsId}: {ex.Message}");
            }
        }

        public async Task ReceiveComment(string newsId, BinhLuanTinTuc comment)
        {
            if (string.IsNullOrEmpty(newsId) || comment == null)
            {
                Console.WriteLine("newsId hoặc comment không hợp lệ.");
                return;
            }

            try
            {
                await Clients.Group(newsId).SendAsync("ReceiveCommentNews", comment);
                Console.WriteLine($"Bình luận đã được gửi đến nhóm {newsId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi bình luận đến nhóm {newsId}: {ex.Message}");
            }
        }
    }

}
