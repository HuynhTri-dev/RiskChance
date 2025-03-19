using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LinhVucsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public LinhVucsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // Danh sách Lĩnh Vực
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "manageBusiness";
            var linhVucs = await _context.LinhVucs.ToListAsync();
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
        public async Task<IActionResult> Create([Bind("IDLinhVuc,TenLinhVuc")] LinhVuc linhVuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(linhVuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(linhVuc);
        }

        // Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var linhVuc = await _context.LinhVucs.FindAsync(id);
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
                .Include(lv => lv.Startups) // Load danh sách Startup theo lĩnh vực
                .FirstOrDefaultAsync(m => m.IDLinhVuc == id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            return View(linhVuc);
        }


        // Xử lý cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IDLinhVuc,TenLinhVuc")] LinhVuc linhVuc)
        {
            if (id != linhVuc.IDLinhVuc) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(linhVuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(linhVuc);
        }

        // Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound("ID lĩnh vực không hợp lệ.");
            }

            var linhVuc = await _context.LinhVucs
                                        .AsNoTracking() // 🚀 Cải thiện hiệu suất khi chỉ đọc dữ liệu
                                        .FirstOrDefaultAsync(lv => lv.IDLinhVuc == id);

            if (linhVuc == null)
            {
                return NotFound("Không tìm thấy lĩnh vực cần xóa.");
            }

            return View(linhVuc);
        }

        // Xử lý xóa lĩnh vực
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var linhVuc = await _context.LinhVucs.FindAsync(id);

            if (linhVuc == null)
            {
                return NotFound("Lĩnh vực này đã bị xóa hoặc không tồn tại.");
            }
            try
            {
                _context.LinhVucs.Remove(linhVuc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "🗑️ Xóa lĩnh vực thành công!";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "❌ Không thể xóa lĩnh vực do có dữ liệu liên quan.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
