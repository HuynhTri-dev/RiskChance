using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Hubs;
using RiskChance.Models;
using RiskChance.Models.ViewModel.User;
using RiskChance.Repositories;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class MessengerController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<TinNhan> _messRepo;
        private readonly IHubContext<MessengerHub> _hubContext;
        private readonly UserManager<NguoiDung> _userManager;

        public MessengerController (ApplicationDBContext context, 
            IHubContext<MessengerHub> hubContext, 
            IRepository<TinNhan> messRepo, 
            UserManager<NguoiDung> userManager)
        {
            _context = context;
            _messRepo = messRepo;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> SendMess(TinNhan mess)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _messRepo.AddAsync(mess);

                await _hubContext.Clients.User(mess.IDNguoiNhan).SendAsync("ReceiveMess", mess);

                return Json(mess);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        public async Task<IActionResult> ListFriend()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var userId = user.Id;

            // Giả sử TinNhan có IDNguoiNhan, IDNguoiGui
            var friends = await _context.TinNhans
                .Where(m => m.IDNguoiGui == userId || m.IDNguoiNhan == userId)
                .Select(m => m.IDNguoiGui == userId ? m.IDNguoiNhan : m.IDNguoiGui)
                .Distinct()
                .ToListAsync();

            var friendUsers = await _context.Users
                .Where(u => friends.Contains(u.Id))
                .ToListAsync();

            var firstFriend = friendUsers.Select(x => x.Id).First();

            var model = new ChatBoxViewModel()
            {
                ListFriend = friendUsers,
                IDNguoiNhan = userId,
            };

            return PartialView("_FriendListPartial", model);
        }

        public async Task<IActionResult> GetMessages(string receiverId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var messages = await _context.TinNhans
                .Where(m => (m.IDNguoiGui == user.Id && m.IDNguoiNhan == receiverId) ||
                            (m.IDNguoiNhan == user.Id && m.IDNguoiGui == receiverId))
                .OrderBy(m => m.ThoiGian)
                .ToListAsync();

            return PartialView("_MessageBoxPartial", messages);
        }
    }
}
        
