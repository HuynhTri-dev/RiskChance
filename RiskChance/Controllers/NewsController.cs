using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.TinTucViewModel;
using RiskChance.Utils;

namespace QuanLyStartup.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public NewsController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Top hahstag
            var topHashtags = await _context.Hashtags
            .Select(h => new
            {
                Name = h.TenHashtag,
                Count = h.TinTucHashtags.Count()
            })
            .OrderByDescending(h => h.Count)
            .Take(5)
            .ToListAsync();

            ViewBag.TopHashtags = topHashtags;

            var user = await _userManager.GetUserAsync(User);
            ViewBag.User = user;

            ViewBag.ActivePage = "news";
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> LoadNews(int page = 1, int pageSize = 6)
        //{
        //    var news = await _context.TinTucs
        //        .OrderByDescending(t => t.NgayDang)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(t => new
        //        {
        //            t.IDTinTuc,
        //            t.NoiDung,
        //            t.ImgTinTuc,
        //            t.NgayDang,
        //            NguoiDang = t.NguoiDung != null ? t.NguoiDung.HoTen : "Ẩn danh" // Lấy tên người đăng
        //        })
        //        .ToListAsync();

        //    return Json(news);
        //}

        //Search
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string keyword)
        {
            var query = _context.TinTucs
                .Include(t => t.NguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.NoiDung.Contains(keyword));
            }

            var result = await query.ToListAsync();
            return View("Index", result); // Load lại trang Index với kết quả tìm kiếm
        }
    }
}
