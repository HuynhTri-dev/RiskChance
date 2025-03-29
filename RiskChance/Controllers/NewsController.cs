using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.TinTucViewModel;
using RiskChance.Repositories;
using RiskChance.Utils;
using System.Text.RegularExpressions;

namespace QuanLyStartup.Controllers
{
    [AllowAnonymous]
    public class NewsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<Hashtag> _hashTagRepo;
        private readonly IRepository<TinTuc> _newsRepo;
        private readonly IRepository<NguoiDung> _userRepo;

        public NewsController(ApplicationDBContext context, 
            UserManager<NguoiDung> userManager, 
            IRepository<Hashtag> hashTagRepo,
            IRepository<TinTuc> newsRepo,
            IRepository<NguoiDung> userRepo)
        {
            _context = context;
            _userManager = userManager;
            _hashTagRepo = hashTagRepo;
            _newsRepo = newsRepo;
            _userRepo = userRepo;
        }

        // Page
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                    ViewBag.User = user;
                }
            }

            var model = new TinTucPageViewModel();

            // Top hahstag
            model.TopHashTag = await _context.TinTucHashtags
                                    .GroupBy(x => x.IDHashtag)          
                                    .Select(g => new
                                    {
                                        HashtagId = g.Key,
                                        Count = g.Count()               
                                    })
                                    .OrderByDescending(x => x.Count)
                                    .Take(5)                             
                                    .Join(_context.Hashtags,
                                          h => h.HashtagId,           
                                          ht => ht.IDHashtag,
                                          (h, ht) => new Hashtag
                                          {
                                              IDHashtag = ht.IDHashtag,
                                              TenHashtag = ht.TenHashtag
                                          })
                                    .ToListAsync();


            // list news
            model.NewsList = await _context.TinTucs
                                .Where(x => x.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                                .OrderByDescending(x => x.NgayDang)
                                .Select(x => new TinTucBoxViewModel
                                {
                                    IDTinTuc = x.IDTinTuc,
                                    Title = x.TieuDe,
                                    ImgTinTuc = x.ImgTinTuc,
                                    NoiDung = TrimHtmlContent(x.NoiDung, 40),
                                    NgayDang = x.NgayDang,
                                    IDNguoiDang = x.IDNguoiDung,
                                    NameNguoiDang = x.NguoiDung.HoTen,
                                    ImgNguoiDang = x.NguoiDung.AvatarUrl
                                })
                                .ToListAsync();
            // top news
            model.TopNews = await _context.TinTucs
                .Where(x => x.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                .OrderByDescending(x => x.BinhLuanTinTucs.Average(dg => dg.DiemDanhGia))
                .Select(x => new TinTucBoxViewModel
                {
                    IDTinTuc = x.IDTinTuc,
                    Title = x.TieuDe,
                    ImgTinTuc = x.ImgTinTuc,
                    NameNguoiDang = x.NguoiDung.HoTen
                })
                .ToListAsync();

            

            ViewBag.ActivePage = "news";

            return View(model);
        }
        
        // hàm này để loại bỏ các thẻ html trong db
        public static string TrimHtmlContent(string? html, int wordLimit)
        {
            if (string.IsNullOrEmpty(html)) return "";

            // Loại bỏ tất cả HTML nhưng giữ xuống dòng (`<br>` -> `\n`)
            string text = Regex.Replace(html, "<br\\s*/?>", "\n");  // Giữ xuống dòng
            text = Regex.Replace(text, "<.*?>", "");  // Xoá thẻ HTML

            // Cắt nội dung theo số từ
            var words = text.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > wordLimit)
            {
                return string.Join(" ", words.Take(wordLimit)) + "..."; // 🔥 Cắt nội dung
            }

            return text;
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
        public async Task<IActionResult> SearchNews(string query)
        {
            var news = await _context.TinTucs
                            .Include(t => t.NguoiDung)
                            .Include(t => t.TinTucHashtags)
                            .Where(t => t.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet &&
                                    (string.IsNullOrEmpty(query) ||
                                     t.TieuDe.Contains(query) ||
                                     (t.NguoiDung != null && t.NguoiDung.HoTen.Contains(query)) ||
                                     (t.TinTucHashtags.Any(h => h.Hashtag != null && h.Hashtag.TenHashtag.Contains(query)))))
                            .OrderByDescending(t => t.NgayDang)
                            .Select(x => new TinTucBoxViewModel
                            {
                                IDTinTuc = x.IDTinTuc,
                                Title = x.TieuDe,
                                ImgTinTuc = x.ImgTinTuc,
                                NoiDung = TrimHtmlContent(x.NoiDung, 40),
                                NgayDang = x.NgayDang,
                                IDNguoiDang = x.IDNguoiDung,
                                NameNguoiDang = x.NguoiDung.HoTen,
                                ImgNguoiDang = x.NguoiDung.AvatarUrl
                            })
                            .ToListAsync();

            return PartialView("_NewsPartial", news);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByHashTag(string query)
        {
            var news = await _context.TinTucs
                            .Include(t => t.NguoiDung)
                            .Include(t => t.TinTucHashtags)
                            .Where(t => t.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet &&
                                    (string.IsNullOrEmpty(query) ||
                                    (t.TinTucHashtags.Any(h => h.Hashtag != null && h.Hashtag.TenHashtag.Contains(query)))))
                            .OrderByDescending(t => t.NgayDang)
                            .Select(x => new TinTucBoxViewModel
                            {
                                IDTinTuc = x.IDTinTuc,
                                Title = x.TieuDe,
                                ImgTinTuc = x.ImgTinTuc,
                                NoiDung = TrimHtmlContent(x.NoiDung, 40),
                                NgayDang = x.NgayDang,
                                IDNguoiDang = x.IDNguoiDung,
                                NameNguoiDang = x.NguoiDung.HoTen,
                                ImgNguoiDang = x.NguoiDung.AvatarUrl
                            })
                            .ToListAsync();

            return PartialView("_NewsPartial", news);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return NotFound();
            }

            var news = await _newsRepo.GetByIdAsync(id);
            if (news == null)
                return NotFound();

            var hashtag = await _context.TinTucHashtags
                            .Where(x => x.IDTinTuc == id)
                            .Select(x => new Hashtag
                            {
                                IDHashtag = x.IDHashtag,
                                TenHashtag = x.Hashtag.TenHashtag
                            })
                            .ToListAsync();


            var model = await _context.TinTucs
                                .Where(x => x.IDTinTuc == id)
                                .Select(x => new TinTucBoxViewModel
                                {
                                    IDTinTuc = x.IDTinTuc,
                                    Title = x.TieuDe,
                                    ImgTinTuc = x.ImgTinTuc,
                                    NoiDung = x.NoiDung,
                                    NgayDang = x.NgayDang,
                                    IDNguoiDang = x.IDNguoiDung,
                                    NameNguoiDang = x.NguoiDung.HoTen,
                                    ImgNguoiDang = x.NguoiDung.AvatarUrl,
                                    Hashtags = hashtag
                                })
                                .FirstOrDefaultAsync();

            return View(model);
        }

    }
}
