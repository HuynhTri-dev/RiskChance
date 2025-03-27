using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.GiayToViewModel;
using RiskChance.Utils;

namespace RiskChance.Areas.Founder.Controllers
{
    [Area("Founder")]
    [Authorize(Roles = "Founder, Admin")]
    public class GiayToesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public GiayToesController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GiayToes/Create
        [HttpGet]
        public async Task<IActionResult> Create(int? startupId)
        {
            // int? startupId = HttpContext.Session.GetInt32("StartupID");

            if (startupId == null)
            {
                TempData["Message"] = "Error create startup";
                return RedirectToAction("Index", "Startup", new { area = "" });
            }

            var listDoc = await _context.GiayTos
                                        .Where(x => x.IDStartup == startupId)
                                        .ToListAsync();

            AddGiayToViewModel addGiayToViewModel = new AddGiayToViewModel
            {
                IdStartup = startupId
            };

            var model = new GiayToPageViewModel()
            {
                AddGiayToViewModel = addGiayToViewModel,
                ListDocs = listDoc
            };

            return View(model);
        }

        // POST: GiayToes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiayToPageViewModel model)
        {
            //int? startupId = HttpContext.Session.GetInt32("StartupID");

            if (model.AddGiayToViewModel.IdStartup == null)
            {
                TempData["Message"] = "Error create startup";
                return RedirectToAction("Index", "Startup", new { area = ""});

            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                model.ListDocs = await _context.GiayTos.Where(g => g.IDStartup == model.AddGiayToViewModel.IdStartup).ToListAsync();
                return RedirectToAction("Create", new { startupId = model.AddGiayToViewModel.IdStartup });
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
                    model.ListDocs = await _context.GiayTos.Where(g => g.IDStartup == model.AddGiayToViewModel.IdStartup).ToListAsync();
                    return RedirectToAction("Create", new { startupId = model.AddGiayToViewModel.IdStartup });
                }
            }

            var giayTo = new GiayTo
            {
                TenGiayTo = model.AddGiayToViewModel.NameDoc,
                NoiDung = model.AddGiayToViewModel.ContentDoc,
                LoaiFile = model.AddGiayToViewModel.TypeDoc,
                IDStartup = model.AddGiayToViewModel.IdStartup,
                FileGiayTo = model.AddGiayToViewModel.FileUrl,
                NgayTao = DateTime.UtcNow
            };

            try
            {
                _context.GiayTos.Add(giayTo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", new { startupId = model.AddGiayToViewModel.IdStartup });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu vào cơ sở dữ liệu: " + ex.Message);
                return RedirectToAction("Create", new { startupId = model.AddGiayToViewModel.IdStartup });
            }
        }

        // GET: GiayToes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? startupId)
        {
            if (startupId == null)
            {
                return NotFound();
            }

            var docs = await _context.GiayTos
                .Where(x => x.IDStartup == startupId)
                .ToListAsync();

            var doc = docs
                .Select(x => new AddGiayToViewModel
                {
                    IdDoc = x.IDGiayTo,
                    NameDoc = x.TenGiayTo,
                    ContentDoc = x.NoiDung,
                    TypeDoc = x.LoaiFile,
                    FileUrl = x.FileGiayTo,
                    IdStartup = x.IDStartup

                })
                .FirstOrDefault();  

            // Gán dữ liệu vào model
            GiayToPageViewModel model = new GiayToPageViewModel
            {
                AddGiayToViewModel = doc,
                ListDocs = docs
            };

            return RedirectToAction("Create", "GiayToes", new { startupId = doc?.IdStartup });
        }


        // POST: GiayToes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GiayToPageViewModel model)
        {
            var formEdit = model.AddGiayToViewModel;

            if (model.AddGiayToViewModel.IdStartup == null)
            {
                ModelState.AddModelError("", "Không tìm thấy StartupID trong session.");
                return RedirectToAction("Index", "Dashboard");
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

            if (model.AddGiayToViewModel.IdStartup != document.IDStartup)
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

            return RedirectToAction("Create", "GiayToes", new { startupId = document.IDStartup });
        }

        // GET: GiayToes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var giayTo = await _context.GiayTos
        //        .Include(g => g.Startup)
        //        .FirstOrDefaultAsync(m => m.IDGiayTo == id);
        //    if (giayTo == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(giayTo);
        //}

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
            return RedirectToAction("Create", "GiayToes", new { startupId = giayTo.IDStartup });
        }
    }
}
