using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    [Authorize(Roles = "Admin")]
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
            var userId = HttpContext.Session.GetString("UserId");
            if (User != null && userId == null)
            {
                var user = await _userManager.GetUserAsync(User);
                HttpContext.Session.SetString("UserId", user.Id);
                ViewBag.User = user;
            }

            ViewBag.TotalStartups = await _context.Startups.CountAsync();
            ViewBag.TotalNews = await _context.TinTucs.CountAsync();
            ViewBag.PendingStartups = await _context.Startups.Where(s => s.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.ChoDuyet).CountAsync();


            var founderRoleId = await _context.Roles
                .Where(r => r.Name == "Founder")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var investorRoleId = await _context.Roles
                .Where(r => r.Name == "Investor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.founderCount = await _context.UserRoles
                .Where(ur => ur.RoleId == founderRoleId)
                .CountAsync();

            ViewBag.investorCount = await _context.UserRoles
                .Where(ur => ur.RoleId == investorRoleId)
                .CountAsync();

            ViewBag.ActiveFeature = "dashboard";
            return View();
        }

        [HttpGet]
        public IActionResult GetStartupData()
        {
            var startupData = _context.Startups
                .Where(s => s.NgayTao.Year == 2025)
                .GroupBy(s => s.NgayTao.Month)
                .Select(g => new
                {
                    month = g.Key,     // Chú ý: dùng chữ thường để phù hợp JSON
                    count = g.Count()
                })
                .ToList();

            // Đảm bảo đủ 12 tháng, gán 0 nếu không có dữ liệu
            var data = Enumerable.Range(1, 12).Select(m => new {
                month = m,
                count = startupData.FirstOrDefault(d => d.month == m)?.count ?? 0
            });

            return Json(data);
        }

        // Phương thức lấy số lượng Startup theo từng lĩnh vực
        public async Task<IActionResult> GetStartupCountByLinhVuc()
        {
            var data = await _context.LinhVucs
                                     .Include(lv => lv.Startups)
                                     .Select(lv => new
                                     {
                                         LinhVuc = lv.TenLinhVuc,
                                         StartupCount = lv.Startups.Count()
                                     })
                                     .ToListAsync();

            return Json(data); // Trả về dữ liệu dưới dạng JSON
        }
        // Phương thức lấy số lượng lượt truy cập theo ngày
        [HttpGet]
        public async Task<JsonResult> GetAccessLogsByDay()
        {
            // Lấy tất cả UserId có vai trò là Founder hoặc Investor
            var founderRoleId = await _context.Roles.Where(r => r.Name == "Founder").Select(r => r.Id).FirstOrDefaultAsync();
            var investorRoleId = await _context.Roles.Where(r => r.Name == "Investor").Select(r => r.Id).FirstOrDefaultAsync();

            var userIds = await _context.UserRoles
                .Where(ur => ur.RoleId == founderRoleId || ur.RoleId == investorRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Lấy lượt truy cập trong năm 2025 theo ngày
            var accessLogs = await _context.AccessLogs
                .Where(log => log.AccessTime.Year == 2025 && userIds.Contains(log.UserId))
                .GroupBy(log => log.AccessTime.Date)
                .Select(g => new {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(accessLogs);
        }

        [HttpGet]
        public async Task<IActionResult> GetContractStatistics()
        {
            var contracts = await _context.HopDongDauTus.ToListAsync();

            var contractStats = contracts
                                .GroupBy(c => new { c.NgayKyKet.Year, c.NgayKyKet.Month, c.TrangThaiKyKet })
                                .Select(g => new
                                {
                                    Year = g.Key.Year,
                                    Month = g.Key.Month,
                                    Status = g.Key.TrangThaiKyKet,
                                    Count = g.Count()
                                })
                                .ToList();

            return Json(contractStats);
        }
    }
}
