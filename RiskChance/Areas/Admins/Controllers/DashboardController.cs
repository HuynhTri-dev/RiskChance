using Microsoft.AspNetCore.Authorization;
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

        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
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
    }
}
