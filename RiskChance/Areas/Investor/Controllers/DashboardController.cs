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

            var totalInvestment = await _context.HopDongDauTus
                                            .Where(x => x.ThanhToan == true && x.IDNguoiDung == userId)
                                            .SumAsync(x => x.TongTien ?? 0);

            var expectProfit = await _context.HopDongDauTus
                                    .Where(x => x.ThanhToan == true && x.IDNguoiDung == userId)
                                    .SumAsync(x => (x.TongTien ?? 0) * (decimal)((x.PhanTramLoiNhuan ?? 0) / 100));

            var profitReceived = await _context.ThanhToanLoiNhuans
                                            //.Include(x => x.HopDongDauTu)
                                            .Where(tt => tt.HopDongDauTu.IDNguoiDung == userId && tt.HopDongDauTu.ThanhToan == true)  // Lọc theo nhà đầu tư và thanh toán đã thực hiện
                                            .SumAsync(tt => tt.SoTien);


            InvestorDashboardViewModel model = new InvestorDashboardViewModel()
            {
                TotalInvestment = totalInvestment,
                ExpectProfit = expectProfit,
                ProfitReceived = profitReceived,
                HopDongDauTus = contracts
            };

            ViewBag.ActiveFeature = "dashboardInvestor";
            return View(model);
        }

        public async Task<IActionResult> PercentInvest()
        {
            var userId = HttpContext.Session.GetString("UserId");

            var investments = await _context.HopDongDauTus
                .Include(x => x.Startup)
                .Where(x => x.IDNguoiDung == userId && x.ThanhToan == true)
                .GroupBy(x => x.Startup.TenStartup)
                .Select(group => new
                {
                    StartupName = group.Key,
                    TotalInvested = group.Sum(x => x.TongTien)
                })
                .ToListAsync();

            var totalInvestedAmount = investments.Sum(x => x.TotalInvested);

            var percentageData = investments.Select(x => new
            {
                x.StartupName,
                x.TotalInvested,
                Percentage = totalInvestedAmount > 0 ? (x.TotalInvested / totalInvestedAmount) * 100 : 0
            });

            return Json(percentageData);
        }
    }
}
