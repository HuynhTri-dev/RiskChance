using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiskChance.Models;
using RiskChance.Models.ViewModel.User;
using RiskChance.Repositories;

namespace RiskChance.Controllers
{
    public class UserController : Controller
    {
        public readonly IRepository<NguoiDung> _userRepo;
        private readonly UserManager<NguoiDung> _userManager;

        public UserController(IRepository<NguoiDung> userRepo, UserManager<NguoiDung> userManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo(string id)
        {
            // Example: Fetch user data from database or service
            var user = await _userManager.FindByIdAsync(id);

            var roles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return NotFound(); // Handle case where user is not found
            }

            // Create a view model with necessary information
            var userViewModel = new UserInfo
            {
                Id = user.Id,
                UserName = user.HoTen,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.AvatarUrl,
                Role = roles.FirstOrDefault(),
            };

            return PartialView("_UserInfoPartial", userViewModel);
        }

    }
}
