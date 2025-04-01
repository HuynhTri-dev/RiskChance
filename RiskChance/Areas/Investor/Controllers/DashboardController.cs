using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Areas.Investor.Models;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.Investor.Controllers
{
    [Area("Investor")]
    [Authorize(Roles = "Investor")]
    public class DashboardController : Controller
    {
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public DashboardController(IRepository<HopDongDauTu> contractRepo, 
            ApplicationDBContext context,
            UserManager<NguoiDung> userManager)
        {
            _contractRepo = contractRepo;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (User != null && userId == null)
            {
                var user = await _userManager.GetUserAsync(User);
                HttpContext.Session.SetString("UserId", user.Id);
                userId = HttpContext.Session.GetString("UserId");
            }


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }


            var contracts = await _context.HopDongDauTus
                                    .Include(x => x.Startup)
                                    .Where(x => x.IDNguoiDung == userId)
                                    .OrderByDescending(x => x.TrangThaiKyKet)
                                    .ToListAsync();

            InvestorDashboardViewModel model = new InvestorDashboardViewModel()
            {
                HopDongDauTus = contracts
            };

            ViewBag.ActiveFeature = "dashboardInvestor";
            return View(model);
        }

    }
}
