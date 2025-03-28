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
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

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
        private readonly IRepository<TinTucHashtag> _tinTucHashtagRepo;

        public NewsController(ApplicationDBContext context, 
                              UserManager<NguoiDung> userManager,
                              IRepository<TinTuc> news,
                              IRepository<Hashtag> hashtag,
                              IRepository<TinTucHashtag> tinTucHashtagRepo)
        {
            _context = context;
            _userManager = userManager;
            _newsRepo = news;
            _hashtagRepo = hashtag;
            _tinTucHashtagRepo = tinTucHashtagRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                ModelState.AddModelError("", "Không thể xác định người dùng.");
                return View();
            }

            var model = new TinTucAddViewModel();
            model.IDNguoiDung = userId;

            return View(model);
        }

        public async Task<IActionResult> Add(TinTucAddViewModel model, IFormFile ImgTinTuc, string hiddenHashtags)
        {
            if (!string.IsNullOrEmpty(hiddenHashtags))
            {
                try
                {
                    model.Hashtags = JsonSerializer.Deserialize<List<string>>(hiddenHashtags);
                }
                catch
                {
                    ModelState.AddModelError("Hashtags", "Dữ liệu hashtag không hợp lệ.");
                }
            }

            // Hien tai ko hop ly xi sua 
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ModelState.AddModelError("", System.Text.Json.JsonSerializer.Serialize(model));

                return View(model);
            }

            if (ImgTinTuc != null)
            {
                try
                {
                    model.ImgTinTuc = await ImageUtil.SaveAsync(ImgTinTuc);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImgTinTuc", "Lỗi khi lưu ảnh: " + ex.Message);
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("ImgTinTuc", "Vui lòng chọn ảnh.");
            }

            var tinTuc = new TinTuc()
            {
                TieuDe = model.TieuDe,
                ImgTinTuc = model.ImgTinTuc,
                NoiDung = model.NoiDung,
                NgayDang = DateTime.Now,
                TrangThaiXetDuyet = TrangThaiXetDuyetEnum.ChoDuyet,
                IDNguoiDung = model.IDNguoiDung
            };

            try
            {
                await _newsRepo.AddAsync(tinTuc);
               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu tin tức vào database: " + ex.Message);
            }

            int tinTucId = tinTuc.IDTinTuc;

            if (model.Hashtags != null && model.Hashtags.Any())
            {
                foreach (var tag in model.Hashtags)
                {
                    try
                    {
                        var existingTag = await _hashtagRepo.GetFirstOrDefaultAsync(h => h.TenHashtag == tag);

                        if (existingTag == null)
                        {
                            existingTag = new Hashtag { TenHashtag = tag };
                            await _hashtagRepo.AddAsync(existingTag);
                        }

                        await _tinTucHashtagRepo.AddAsync(new TinTucHashtag
                        {
                            IDTinTuc = tinTucId,
                            IDHashtag = existingTag.IDHashtag
                        });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Lỗi khi xử lý hashtag '{tag}': {ex.Message}");
                        return View(model);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Hashtags", "Cần nhập ít nhất một hashtag.");
            }

            return RedirectToAction("Index", "News", new { area = "" });
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
        [ValidateAntiForgeryToken]
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
