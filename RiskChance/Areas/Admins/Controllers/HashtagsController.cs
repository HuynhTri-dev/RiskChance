using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.Admins;
using RiskChance.Repositories;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class HashtagsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<Hashtag> _hashTagRepo;
        public HashtagsController(ApplicationDBContext context,
                                  IRepository<Hashtag> hashTagRepo)
        {
            _context = context;
            _hashTagRepo = hashTagRepo;
        }

        // GET: Admins/Hashtags
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "hashtagManage";
            return View(await _context.Hashtags.ToListAsync());
        }

        // GET: Admins/Hashtags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hashTag = await _hashTagRepo
                                .GetFirstOrDefaultAsync(m => m.IDHashtag == id);
            if (hashTag == null)
            {
                return NotFound();
            }

            var newlist = await _context.TinTucHashtags
                                    .Include(x => x.TinTuc)
                                    .Where(x => x.IDHashtag == id)
                                    .Select(x => new TinTuc
                                    {
                                        IDTinTuc = x.IDTinTuc,
                                        TieuDe = x.TinTuc.TieuDe,
                                        NgayDang = x.TinTuc.NgayDang
                                    })
                                    .ToListAsync();

            DetailsHashtagViewModel model = new DetailsHashtagViewModel()
            {
                TinTucs = newlist,
                hashtag = hashTag
            };

            return View(model);
        }

        // GET: Admins/Hashtags/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hashtag hashtag)
        {
            if (ModelState.IsValid)
            {
                await _hashTagRepo.AddAsync(hashtag);
                return RedirectToAction(nameof(Index));
            }
            return View(hashtag);
        }

        // GET: Admins/Hashtags/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hashtag = await _hashTagRepo.GetByIdAsync(id);

            if (hashtag == null) return NotFound();

            return View(hashtag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hashtag hashtag)
        {
            if (id != hashtag.IDHashtag)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _hashTagRepo.UpdateAsync(hashtag);
                return RedirectToAction(nameof(Index));
            }

            return View(hashtag);
        }

        // POST: Admins/Hashtags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hashtag = await _context.Hashtags.FindAsync(id);
            if (hashtag != null)
            {
                _context.Hashtags.Remove(hashtag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SearchHashtags(string keyword)
        {
            var hashtags = await _context.Hashtags
                                .Where(x => string.IsNullOrEmpty(keyword) || x.TenHashtag.ToLower().Contains(keyword.ToLower()))
                                .ToListAsync();

            return View("Index", hashtags);
        }
    }
}
