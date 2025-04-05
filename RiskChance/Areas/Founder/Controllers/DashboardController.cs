using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Areas.Founder.ViewModels;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.Founder.Controllers
{
    [Area("Founder")]
    [Authorize(Roles = "Founder")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<DanhGiaStartup> _commentStartupRepo;

        public DashboardController(ApplicationDBContext context,
                                   UserManager<NguoiDung> userManager,
                                   IRepository<DanhGiaStartup> commentStartup)
        {
            _context = context;
            _userManager = userManager;
            _commentStartupRepo = commentStartup;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (User != null && userId == null)
            {
                var user = await _userManager.GetUserAsync(User);
                HttpContext.Session.SetString("UserId", user.Id);
                userId = HttpContext.Session.GetString("UserId");
            }

            return await LoadDashboard(id);
        }

        [HttpPost]
        public IActionResult Index(DashboardViewModel model)
        {
            return RedirectToAction("Index", new { id = model.SelectedStartupId });
        }
        private async Task<IActionResult> LoadDashboard(int? id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            //var user = await _userManager.GetUserAsync(User);
            //if (user == null) return Unauthorized();

            var startupList = await _context.Startups
                                .Where(s => s.IDNguoiDung == userId)
                                .Select(x => new SelectListItem
                                {
                                    Value = x.IDStartup.ToString(),
                                    Text = x.TenStartup
                                })
                                .ToListAsync();

            if (!startupList.Any())
            {
                return View();
            }

            int selectedId = id ?? (startupList.Any() ? int.Parse(startupList.First().Value) : 0);
            var startupInfo = await LoadStartupInfo(selectedId);

            var coInvestor = startupInfo.HopDongDauTus.DistinctBy(hd => hd.IDNguoiDung).Count();
            var totalInvestment = startupInfo.HopDongDauTus
                                .Where(x => x.TrangThaiKyKet == TrangThaiKyKetEnum.DaDuyet && x.ThanhToan == true)
                                .Sum(x => x.TongTien);

            var interactView = await _context.DanhGiaStartups.CountAsync(x => x.IDStartup == selectedId);

            DashboardViewModel viewModel = new DashboardViewModel()
            {
                startupSelectList = startupList,
                SelectedStartupId = selectedId,
                SelectStartup = startupInfo,
                CoInvestors = coInvestor,
                TotalInvestment = totalInvestment,
                InteractView = interactView
            };

            ViewBag.FeatureActive = "dashboardFounder";
            return View(viewModel);
        }
        public async Task<Startup> LoadStartupInfo(int? id)
        {
            var model = await _context.Startups
                                    .Include(x => x.HopDongDauTus)
                                        .ThenInclude(hd => hd.NguoiDung)
                                    .Include(x => x.GiayTos)
                                    .Include(x => x.DanhGiaStartups)
                                    .Include(x => x.LinhVuc)
                                    .FirstOrDefaultAsync(x => x.IDStartup == id);
            return model;
        }
    }
}
