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
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tinTuc = await _context.TinTucs.Include(t => t.TinTucHashtags).ThenInclude(th => th.Hashtag).FirstOrDefaultAsync(t => t.IDTinTuc == id);
            if (tinTuc == null) return NotFound();

            return View(new TinTucAddViewModel
            {
                NoiDung = tinTuc.NoiDung,
                ImgTinTuc = tinTuc.ImgTinTuc,
                Hashtags = tinTuc.TinTucHashtags.Select(h => h.Hashtag.TenHashtag).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TinTucAddViewModel model, IFormFile? imageUrl)
        {
            var tinTuc = await _context.TinTucs.Include(t => t.TinTucHashtags).FirstOrDefaultAsync(t => t.IDTinTuc == model.IDTinTuc);
            if (tinTuc == null) return NotFound();

            tinTuc.NoiDung = model.NoiDung;
            if (imageUrl != null) tinTuc.ImgTinTuc = await ImageUtil.SaveAsync(imageUrl);

            tinTuc.TinTucHashtags.Clear();
            if (model.Hashtags?.Any() == true)
            {
                foreach (var tag in model.Hashtags)
                {
                    var existingTag = await _context.Hashtags.FirstOrDefaultAsync(h => h.TenHashtag == tag)
                                     ?? new Hashtag { TenHashtag = tag };

                    _context.Hashtags.Add(existingTag);
                    await _context.SaveChangesAsync();

                    tinTuc.TinTucHashtags.Add(new TinTucHashtag { IDHashtag = existingTag.IDHashtag });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Xoa
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc == null) return NotFound();

            _context.TinTucs.Remove(tinTuc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
