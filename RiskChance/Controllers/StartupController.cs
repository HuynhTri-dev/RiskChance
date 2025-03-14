using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;

namespace QuanLyStartup.Controllers
{
    public class StartupController : Controller
    {
        private readonly ApplicationDBContext _context;

        public StartupController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.ActivePage = "startup";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int page = 1)
        {
            var pageSize = 12;
            var startups = await _context.Startups
                .Skip((page - 1) * pageSize)
                .Take(pageSize + 1) // Lấy thêm 1 để kiểm tra còn dữ liệu không
                .ToListAsync();

            bool hasMore = startups.Count > pageSize;
            if (hasMore) startups.RemoveAt(startups.Count - 1); // Xóa phần tử thừa

            return Json(new { startups, hasMore });
        }
    }
}
