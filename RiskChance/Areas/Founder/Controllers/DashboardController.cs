using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Areas.Founder.ViewModels;
using RiskChance.Data;
using RiskChance.Models;

namespace RiskChance.Areas.Founder.Controllers
{
    [Area("Founder")]
    [Authorize(Roles = "Founder")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public DashboardController(ApplicationDBContext context, 
                                   UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var startups = await _context.Startups
                                .Where(s => s.IDNguoiDung == user.Id)
                                .Select(x => new SelectListItem
                                {
                                    Value = x.IDStartup.ToString(),
                                    Text = x.TenStartup
                                })
                                .ToListAsync();
            int defaultStartupId = startups.Any() ? int.Parse(startups.First().Value) : 0;

            DashboardViewModel viewModel = new DashboardViewModel()
            {
                startupSelectList = startups,
                SelectedStartupId = defaultStartupId
            };

            ViewBag.FeatureActive = "dashboardFounder";
            return View(viewModel);
        }
    }
}
