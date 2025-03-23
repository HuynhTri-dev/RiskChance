using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Models.ViewModel.TinTucViewModel;
using RiskChance.Models;
using RiskChance.Utils;
using RiskChance.Data;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using RiskChance.Repositories;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<TinTuc> _newsRepo;
        private readonly IRepository<Hashtag> _hashtagRepo;
        private readonly IRepository<TinTucHashtag> _hashtagHashtagRepo;

        public NewsController(ApplicationDBContext context, 
                              UserManager<NguoiDung> userManager,
                              IRepository<TinTuc> news,
                              IRepository<Hashtag> hashtag,
                              IRepository<TinTucHashtag> hashtagHashtagRepo)
        {
            _context = context;
            _userManager = userManager;
            _newsRepo = news;
            _hashtagRepo = hashtag;
            _hashtagHashtagRepo = hashtagHashtagRepo;
        }

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
                    try
                    {
                        model.ImgTinTuc = await ImageUtil.SaveAsync(imageUrl);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Lỗi khi lưu ảnh: " + ex.Message);
                        return View(model);
                    }
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
                        var existingTag = await _hashtagRepo.GetFirstOrDefaultAsync(h => h.TenHashtag == tag);
                        // neu ko co thi them moi
                        if (existingTag == null)
                        {
                            existingTag = new Hashtag { TenHashtag = tag };

                            await _hashtagRepo.AddAsync(existingTag);
                        }

                        await _hashtagHashtagRepo.AddAsync(new TinTucHashtag
                        {
                            IDHashtag = existingTag.IDHashtag
                        });
                    }
                }

                await _newsRepo.AddAsync(tinTuc);
                return RedirectToAction("Index", "News", new { areas = ""});
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
    }
}
