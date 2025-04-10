using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RiskChance.Data;
using Microsoft.AspNetCore.Identity;
using RiskChance.Models;
using Microsoft.AspNetCore.Authorization;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LoginController : Controller
    {
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly UserManager<NguoiDung> _userManager;

        public LoginController(SignInManager<NguoiDung> signInManager, UserManager<NguoiDung> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                ViewBag.Error = "Wrong password or email!";
                return View();
            }

            // Kiểm tra xem user có thuộc nhóm Admin không
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                ViewBag.Error = "You do not have access!";
                return View();
            }

            // Kiểm tra mật khẩu
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admins" }   );
            }

            ViewBag.Error = "Wrong password or email!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
