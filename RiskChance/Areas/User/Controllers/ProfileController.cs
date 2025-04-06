using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;
using RiskChance.Utils;
using System.Data;
using RiskChance.Models.ViewModel.User;
using System.Security.Claims;
namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly IRepository<NguoiDung > _userRepo;
        private readonly ApplicationDBContext _context;

        public ProfileController(UserManager<NguoiDung> userManager, 
                                 SignInManager<NguoiDung> signInManager,
                                 IRepository<NguoiDung> userRepo,
                                 ApplicationDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepo = userRepo;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.FeatureActive = "profile";

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleAccount = roles.FirstOrDefault() ?? "No Role";

            var model = new ProfileViewModel
            {
                UserID = user.Id,
                FullName = user.HoTen,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                RoleAccount = roleAccount
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // Tìm người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Cập nhật thông tin người dùng
            user.HoTen = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;


            // Thực hiện cập nhật
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Update failed!");
                return RedirectToAction("Index");
            }

            // Làm mới thông tin đăng nhập
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Profile updated successfully!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile? AvatarUrl)
        {
            if (AvatarUrl == null)
            {
                ModelState.AddModelError("", "Update failed!");
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                ModelState.AddModelError("", "Update failed!");
                return RedirectToAction("Index");
            }

            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "Update failed!");
                return RedirectToAction("Index");
            }

            string? fileUrl = null;

            if (AvatarUrl != null && AvatarUrl.Length > 0)
            {
                try
                {
                    fileUrl = await ImageUtil.SaveAsync(AvatarUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cannot upload your image!");
                    return RedirectToAction("Index");
                }
            }

            if (fileUrl != null)
            {
                user.AvatarUrl = fileUrl;

                _context.SaveChanges();
                TempData["Success"] = "Avatar updated successfully!";
                return RedirectToAction("Index");
            }
            else { return RedirectToAction("Index"); }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to delete profile.";
                return RedirectToAction("Index");
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
