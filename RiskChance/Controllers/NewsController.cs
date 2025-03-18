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

        public async Task<IActionResult> Index()
        {
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


        // Them
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TinTucAddViewModel model, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    model.ImgTinTuc = await ImageUtil.SaveAsync(imageUrl);
                }

                var tinTuc = new TinTuc()
                {
                    ImgTinTuc = model.ImgTinTuc,
                    NoiDung = model.NoiDung,
                    NgayDang = DateTime.Now,
                    IDNguoiDung = (await _userManager.GetUserAsync(User))?.Id,
                };

                // cap nhat hashtag va hashtag tin tuc
                if (model.Hashtags != null && model.Hashtags.Any())
                {
                    foreach (var tag in model.Hashtags)
                    {
                        var existingTag = _context.Hashtags.FirstOrDefault(h => h.TenHashtag == tag);
                        // neu ko co thi them moi
                        if (existingTag == null)
                        {
                            existingTag = new Hashtag { TenHashtag = tag };
                            _context.Hashtags.Add(existingTag);
                            _context.SaveChanges();
                        }

                        tinTuc.TinTucHashtags.Add(new TinTucHashtag
                        {
                            IDHashtag = existingTag.IDHashtag
                        });
                    }
                }


                _context.TinTucs.Add(tinTuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(model);
        }

        // Sua
        // Xoa
    }
}
