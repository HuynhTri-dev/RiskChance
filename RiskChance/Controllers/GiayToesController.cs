using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.GiayToViewModel;
using RiskChance.Utils;

namespace RiskChance.Controllers
{
    public class GiayToesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public GiayToesController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GiayToes
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.GiayTos.Include(g => g.Startup);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: GiayToes/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayTo = await _context.GiayTos
                .Include(g => g.Startup)
                .FirstOrDefaultAsync(m => m.IDGiayTo == id);
            if (giayTo == null)
            {
                return NotFound();
            }

            return View(giayTo);
        }

        // GET: GiayToes/Create
        [Authorize(Roles = "Founder, Admin")]
        public async Task<IActionResult> Create()
        {
            int? startupId = HttpContext.Session.GetInt32("StartupID");

            if (startupId == null)
            {
                return RedirectToAction("Add", "Startup");
            }

            var listDoc = await _context.GiayTos
                                        .Where(x => x.IDStartup == startupId)
                                        .ToListAsync();

            var model = new GiayToPageViewModel()
            {
                AddGiayToViewModel = new AddGiayToViewModel(),
                ListDocs = listDoc
            };

            return View(model);
        }

        // POST: GiayToes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Founder, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiayToPageViewModel model)
        {
            int? startupId = HttpContext.Session.GetInt32("StartupID");

            if (startupId == null)
            {
                ModelState.AddModelError("", "Không tìm thấy StartupID trong session.");
                return RedirectToAction("Add", "Startup");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                    ModelState.AddModelError("", error.ErrorMessage); // Thêm lỗi vào ModelState để hiển thị trên View
                }
                model.ListDocs = await _context.GiayTos.Where(g => g.IDStartup == startupId).ToListAsync();
                return View(model);
            }


            if (model.AddGiayToViewModel.GetFile != null && model.AddGiayToViewModel.GetFile.Length > 0)
            {
                try
                {
                    model.AddGiayToViewModel.FileUrl = await DocumentUtil.SaveAsync(model.AddGiayToViewModel.GetFile);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu tệp: " + ex.Message);
                    model.ListDocs = await _context.GiayTos.Where(g => g.IDStartup == startupId).ToListAsync();
                    return View(model);
                }
            }

            var giayTo = new GiayTo
            {
                TenGiayTo = model.AddGiayToViewModel.NameDoc,
                NoiDung = model.AddGiayToViewModel.ContentDoc,
                LoaiFile = model.AddGiayToViewModel.TypeDoc,
                IDStartup = startupId.Value,
                FileGiayTo = model.AddGiayToViewModel.FileUrl,
                NgayTao = DateTime.UtcNow
            };

            try
            {
                _context.GiayTos.Add(giayTo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving to DB: " + ex.Message);
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu vào cơ sở dữ liệu: " + ex.Message);
                return View(model);
            }
        }


        // GET: GiayToes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayTo = await _context.GiayTos.FindAsync(id);
            if (giayTo == null)
            {
                return NotFound();
            }
            ViewData["IDStartup"] = new SelectList(_context.Startups, "IDStartup", "IDNguoiDung", giayTo.IDStartup);
            return View(giayTo);
        }

        // POST: GiayToes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GiayToPageViewModel model)
        {
            int? startupId = HttpContext.Session.GetInt32("StartupID");

            var formEdit = model.AddGiayToViewModel;

            if (startupId == null)
            {
                ModelState.AddModelError("", "Không tìm thấy StartupID trong session.");
                return RedirectToAction("Index", "Startup");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy tài liệu cũ từ database
            var document = await _context.GiayTos.FindAsync(formEdit.IdDoc);
            if (document == null)
            {
                return NotFound();
            }

            if (startupId != document.IDStartup)
            {
                ModelState.AddModelError("", "Không có quyền xử lý");
            }

            // Cập nhật thông tin tài liệu
            document.TenGiayTo = formEdit.NameDoc;
            document.LoaiFile = formEdit.TypeDoc;
            document.NoiDung = formEdit.ContentDoc;

            // Xử lý file mới nếu có
            if (formEdit.GetFile != null && document.FileGiayTo != formEdit.FileUrl)
            {
                document.FileGiayTo = await DocumentUtil.SaveAsync(formEdit.GetFile);
            }

            // Lưu thay đổi
            _context.Update(document);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "GiayToes");
        }

        // GET: GiayToes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayTo = await _context.GiayTos
                .Include(g => g.Startup)
                .FirstOrDefaultAsync(m => m.IDGiayTo == id);
            if (giayTo == null)
            {
                return NotFound();
            }

            return View(giayTo);
        }

        // POST: GiayToes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giayTo = await _context.GiayTos.FindAsync(id);
            if (giayTo != null)
            {
                _context.GiayTos.Remove(giayTo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Create", "GiayToes");
        }

        private bool GiayToExists(int id)
        {
            return _context.GiayTos.Any(e => e.IDGiayTo == id);
        }
    }
}
