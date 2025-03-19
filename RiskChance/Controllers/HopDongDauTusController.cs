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

namespace RiskChance.Controllers
{
    [Authorize]
    public class HopDongDauTusController : Controller
    {
        private readonly ApplicationDBContext _context;

        public HopDongDauTusController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: HopDongDauTus
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.HopDongDauTus.Include(h => h.NguoiDung).Include(h => h.Startup);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: HopDongDauTus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDongDauTu = await _context.HopDongDauTus
                .Include(h => h.NguoiDung)
                .Include(h => h.Startup)
                .FirstOrDefaultAsync(m => m.IDHopDong == id);
            if (hopDongDauTu == null)
            {
                return NotFound();
            }

            return View(hopDongDauTu);
        }

        // GET: HopDongDauTus/Create
        public IActionResult Create()
        {
            ViewData["IDNguoiDung"] = new SelectList(_context.NguoiDungs, "Id", "Id");
            ViewData["IDStartup"] = new SelectList(_context.Startups, "IDStartup", "IDNguoiDung");
            return View();
        }

        // POST: HopDongDauTus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDHopDong,AnhXacNhan,NgayKyKet,TongTien,PhanTramLoiNhuan,NoiDungHopDong,TrangThai,IDStartup,IDNguoiDung")] HopDongDauTu hopDongDauTu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hopDongDauTu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDNguoiDung"] = new SelectList(_context.NguoiDungs, "Id", "Id", hopDongDauTu.IDNguoiDung);
            ViewData["IDStartup"] = new SelectList(_context.Startups, "IDStartup", "IDNguoiDung", hopDongDauTu.IDStartup);
            return View(hopDongDauTu);
        }

        // GET: HopDongDauTus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDongDauTu = await _context.HopDongDauTus.FindAsync(id);
            if (hopDongDauTu == null)
            {
                return NotFound();
            }
            ViewData["IDNguoiDung"] = new SelectList(_context.NguoiDungs, "Id", "Id", hopDongDauTu.IDNguoiDung);
            ViewData["IDStartup"] = new SelectList(_context.Startups, "IDStartup", "IDNguoiDung", hopDongDauTu.IDStartup);
            return View(hopDongDauTu);
        }

        // POST: HopDongDauTus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IDHopDong,AnhXacNhan,NgayKyKet,TongTien,PhanTramLoiNhuan,NoiDungHopDong,TrangThai,IDStartup,IDNguoiDung")] HopDongDauTu hopDongDauTu)
        {
            if (id != hopDongDauTu.IDHopDong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hopDongDauTu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongDauTuExists(hopDongDauTu.IDHopDong))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDNguoiDung"] = new SelectList(_context.NguoiDungs, "Id", "Id", hopDongDauTu.IDNguoiDung);
            ViewData["IDStartup"] = new SelectList(_context.Startups, "IDStartup", "IDNguoiDung", hopDongDauTu.IDStartup);
            return View(hopDongDauTu);
        }

        // GET: HopDongDauTus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDongDauTu = await _context.HopDongDauTus
                .Include(h => h.NguoiDung)
                .Include(h => h.Startup)
                .FirstOrDefaultAsync(m => m.IDHopDong == id);
            if (hopDongDauTu == null)
            {
                return NotFound();
            }

            return View(hopDongDauTu);
        }

        // POST: HopDongDauTus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hopDongDauTu = await _context.HopDongDauTus.FindAsync(id);
            if (hopDongDauTu != null)
            {
                _context.HopDongDauTus.Remove(hopDongDauTu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HopDongDauTuExists(int id)
        {
            return _context.HopDongDauTus.Any(e => e.IDHopDong == id);
        }
    }
}
