using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.Admins;
namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class ManagerUserController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManagerUserController(
            ApplicationDBContext context,
            UserManager<NguoiDung> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var userRolesViewModel = new List<UserManagerViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserManagerViewModel
                {
                    Id = user.Id,
                    UserName = user.HoTen,
                    Email = user.Email,
                    Roles = userRoles.ToList()
                });
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.AllRoles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            ViewBag.ActiveFeature = "userManage";


            return View(userRolesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                TempData["Message"] = "Unknow id and role of user";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Message"] = "Don't find the user";
                return RedirectToAction("Index");
            }

            // Lấy vai trò hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Xóa tất cả vai trò hiện tại
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["Message"] = "Cannot delete that user";
                return RedirectToAction("Index");
            }

            // Gán vai trò mới
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                TempData["StatusMessage"] = "Add new roles";
                return RedirectToAction("Index");
            }

            TempData["StatusMessage"] = "Update role success";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SearchUser(string keyword)
        {
            var users = await _userManager.Users
                                .Where(x => string.IsNullOrEmpty(keyword) || x.HoTen.ToLower().Contains(keyword.ToLower()))
                                .ToListAsync();

            var userRolesViewModel = new List<UserManagerViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserManagerViewModel
                {
                    Id = user.Id,
                    UserName = user.HoTen,
                    Email = user.Email,
                    Roles = userRoles.ToList()
                });
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.AllRoles = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            return View("Index", userRolesViewModel);
        }
    }
}
