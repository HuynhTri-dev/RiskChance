using System.Diagnostics;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.HomeViewModel;
using RiskChance.Models.ViewModel.TinTucViewModel;

namespace QuanLyStartup.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly UserManager<NguoiDung> _userManager;

    public HomeController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

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

        HomeViewModel model = new HomeViewModel();

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
                                .Take(3)
                                .ToListAsync();

        model.StartupList = await _context.Startups
                                    .Where(s => s.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                                    .OrderByDescending(s => s.NgayTao)
                                    .Include(s => s.LinhVuc)
                                    .Take(12)
                                    .ToListAsync();



        ViewBag.ActivePage = "home";
        return View(model);
    }

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
