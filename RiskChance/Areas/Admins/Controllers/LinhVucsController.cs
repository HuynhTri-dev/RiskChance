using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public class LinhVucsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<LinhVuc> _businessRepo;

        public LinhVucsController(ApplicationDBContext context,
            IRepository<LinhVuc> businessRepo)
        {
            _context = context;
            _businessRepo = businessRepo;
        }

        // Danh sách Lĩnh Vực
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "manageBusiness";
            var linhVucs = await _businessRepo.GetAllAsync();
            return View(linhVucs);
        }

        // Hiển thị form tạo mới
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LinhVuc linhVuc)
        {
            if (ModelState.IsValid)
            {
                await _businessRepo.AddAsync(linhVuc);
                return RedirectToAction(nameof(Index));
            }
            return View(linhVuc);
        }

        // Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var linhVuc = await _businessRepo.GetByIdAsync(id);
            if (linhVuc == null) return NotFound();
            return View(linhVuc);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var linhVuc = await _context.LinhVucs
                .Include(lv => lv.Startups)
                .FirstOrDefaultAsync(m => m.IDLinhVuc == id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            //DetailBusinessViewModel model = new DetailBusinessViewModel();

            return View(linhVuc);
        }


        // Xử lý cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LinhVuc linhVuc)
        {
            if (id != linhVuc.IDLinhVuc) return NotFound();
            if (ModelState.IsValid)
            {
                await _businessRepo.UpdateAsync(linhVuc);
                return RedirectToAction(nameof(Index));
            }
            return View(linhVuc);
        }

        // Xử lý xóa lĩnh vực
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var linhVuc = await _businessRepo.GetByIdAsync(id);

            if (linhVuc == null)
            {
                return NotFound();
            }
            try
            {
                await _businessRepo.DeleteAsync(id);
                TempData["SuccessMessage"] = "Delete success";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SearchBusiness(string keyword)
        {
            var business = await _context.LinhVucs
                                .Where(x => string.IsNullOrEmpty(keyword) || x.TenLinhVuc.ToLower().Contains(keyword.ToLower()))
                                .ToListAsync();

            return View("Index", business);
        }
    }
}
