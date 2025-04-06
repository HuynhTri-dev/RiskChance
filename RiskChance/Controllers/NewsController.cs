using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
        private const int PageSize = 9;

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
            model.NewsList = await GetPagedAsync(1, PageSize);
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

        public async Task<IEnumerable<TinTucBoxViewModel>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _context.TinTucs
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
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> LoadMore(int pageIndex = 1)
        {
            var news = await GetPagedAsync(pageIndex, PageSize);
            return PartialView("_NewsPartial", news);
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

            var news = await _context.TinTucs
                            .Include(x => x.BinhLuanTinTucs)    
                                .ThenInclude(x => x.NguoiDung)
                            .Include(x => x.NguoiDung)
                            .Include(x => x.TinTucHashtags)
                                .ThenInclude(x => x.Hashtag)
                            .FirstOrDefaultAsync(x => x.IDTinTuc == id);

            if (news == null) return NotFound();

            DetailNewsViewModel model = new DetailNewsViewModel()
            {
                TinTuc = news,
                BinhLuanTinTuc = new BinhLuanTinTuc()
                {
                    IDTinTuc = news.IDTinTuc
                }
            };

            return View(model);
        }

    }
}
