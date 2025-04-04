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
        [HttpPost]
        public async Task<IActionResult> SendMess(string id, string mess)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var messenger = new TinNhan()
                {
                    NoiDung = mess,
                    ThoiGian = DateTime.Now,
                    TrangThai = "Chưa đọc",
                    IDNguoiGui = HttpContext.Session.GetString("UserId"),
                    IDNguoiNhan = id
                };

                await _messRepo.AddAsync(messenger);

                await _hubContext.Clients.User(id).SendAsync("ReceiveMess", messenger);

                return Json(messenger);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
        [HttpGet]
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

            return PartialView("~/Views/Shared/Messenger/_FriendListPartial.cshtml", friendUsers);
        }
        [HttpGet]
        public async Task<IActionResult> ListMessages(string receiverId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var messages = await _context.TinNhans
                .Where(m => (m.IDNguoiGui == user.Id && m.IDNguoiNhan == receiverId) ||
                            (m.IDNguoiNhan == user.Id && m.IDNguoiGui == receiverId))
                .OrderBy(m => m.ThoiGian)
                .ToListAsync();

            return PartialView("~/Views/Shared/Messenger/_MessListPartial.cshtml", messages);
        }

        [HttpGet]
        public async Task<IActionResult> FindName(string name)
        {
            var userList = await _context.NguoiDungs
                .Where(u => u.HoTen.Contains(name))
                .ToListAsync();

            return PartialView("~/Views/Shared/Messenger/_FriendListPartial.cshtml", userList);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo(string id)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Id == id);

            return PartialView("~/Views/Shared/Messenger/_InforReceivePartial.cshtml", user);
        }
    }
}
        
