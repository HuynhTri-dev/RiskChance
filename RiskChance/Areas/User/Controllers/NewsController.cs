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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

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
        public IActionResult Add()
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

        public async Task<IActionResult> Add(TinTucAddViewModel model, IFormFile? ImgTinTuc, string? hiddenHashtags)
        {
            if (!string.IsNullOrEmpty(hiddenHashtags))
            {
                try
                {
                    model.Hashtags = JsonSerializer.Deserialize<List<string>>(hiddenHashtags);
                }
                catch
                {
                    ModelState.AddModelError("Hashtags", "None hashtag are available");
                }
            }
 
            if (!ModelState.IsValid)
            {
                //foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                //{
                //   
                //}

                //ModelState.AddModelError("", System.Text.Json.JsonSerializer.Serialize(model));
                ModelState.AddModelError("", "Error post news");

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
                    ModelState.AddModelError("ImgTinTuc", "Error when upload picture: " + ex.Message);
                    return View(model);
                }
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
                ModelState.AddModelError("", "Error create news: " + ex.Message);
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
                        ModelState.AddModelError("", $"Error solve hashtag '{tag}': {ex.Message}");
                        return View(model);
                    }
                }
            }

            return RedirectToAction("Index", "News", new { area = "" });
        }


        // Danh sach cac news cua nguoi dung da dang
        public async Task<IActionResult> UserIndex()
        {
            ViewBag.FeatureActive = "myPost";
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                ModelState.AddModelError("", "Cannot Find The User");
                return View();
            }

            var news = await _context.TinTucs
                            .Where(t => t.IDNguoiDung == userId)
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

            return View(news);
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

        // Sua
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tinTuc = await _context.TinTucs.Include(t => t.TinTucHashtags).ThenInclude(th => th.Hashtag).FirstOrDefaultAsync(t => t.IDTinTuc == id);
            if (tinTuc == null) return NotFound();

            return View(new TinTucAddViewModel
            {
                IDTinTuc = id,
                TieuDe = tinTuc.TieuDe,
                IDNguoiDung = tinTuc.IDNguoiDung,
                NoiDung = tinTuc.NoiDung,
                ImgTinTuc = tinTuc.ImgTinTuc,
                Hashtags = tinTuc.TinTucHashtags.Select(h => h.Hashtag.TenHashtag).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TinTucAddViewModel model, IFormFile? ImgTinTuc, string? hiddenHashtags)
        {
            var tinTuc = await _context.TinTucs
                .Include(t => t.TinTucHashtags)
                .FirstOrDefaultAsync(t => t.IDTinTuc == model.IDTinTuc);

            if (tinTuc == null) return RedirectToAction(nameof(UserIndex));

            if (!string.IsNullOrEmpty(hiddenHashtags))
            {
                try
                {
                    model.Hashtags = JsonSerializer.Deserialize<List<string>>(hiddenHashtags);
                }
                catch
                {
                    ModelState.AddModelError("Hashtags", "None hashtag are available");
                }
            }

            tinTuc.TieuDe = model.TieuDe;
            tinTuc.NoiDung = model.NoiDung;
            tinTuc.NgayDang = DateTime.Now;

            if (ImgTinTuc != null)
            {
                try
                {
                    tinTuc.ImgTinTuc = await ImageUtil.SaveAsync(ImgTinTuc);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImgTinTuc", "Error when upload picture: " + ex.Message);
                    return View(model);
                }
            }

            int idTinTuc = tinTuc.IDTinTuc;

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

                        var existingTinTucHashtag = await _tinTucHashtagRepo
                            .GetFirstOrDefaultAsync(th => th.IDTinTuc == idTinTuc && th.IDHashtag == existingTag.IDHashtag);

                        if (existingTinTucHashtag != null) continue;

                        await _tinTucHashtagRepo.AddAsync(new TinTucHashtag
                        {
                            IDTinTuc = idTinTuc,
                            IDHashtag = existingTag.IDHashtag
                        });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error solve hashtag '{tag}': {ex.Message}");
                        return View(model);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserIndex));
        }

        // Xoa
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                
                var tinTuc = await _context.TinTucs
                    .Include(t => t.BinhLuanTinTucs)
                    .FirstOrDefaultAsync(t => t.IDTinTuc == id);

                if (tinTuc == null) return NotFound();

                _context.BinhLuanTinTucs.RemoveRange(tinTuc.BinhLuanTinTucs);

                await _newsRepo.DeleteAsync(id);

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Delete Unsuccess!";
                return RedirectToAction(nameof(UserIndex));
            }
            return RedirectToAction(nameof(UserIndex));
        }
    }
}
