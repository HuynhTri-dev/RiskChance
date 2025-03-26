using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Models.ViewModel.StartupViewModel;
using RiskChance.Models;
using RiskChance.Utils;
using RiskChance.Data;
using RiskChance.Repositories;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using RiskChance.Hubs;

namespace RiskChance.Areas.Founder.Controllers
{
    [Area("Founder")]
    [Authorize(Roles = "Founder, Admin")]
    public class StartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<Startup> _startupRepo;
        private readonly IRepository<GiayTo> _docRepo;
        private readonly IRepository<DanhGiaStartup> _comStartupRepo;
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private readonly IHubContext<StatusStartupHub> _hubContext;

        public StartupController(ApplicationDBContext context,
                                 UserManager<NguoiDung> userManager,
                                 IRepository<Startup> startupRepo,
                                 IRepository<GiayTo> docRepo,
                                 IRepository<DanhGiaStartup> comStartupRepo,
                                 IRepository<HopDongDauTu> contractRepo,
                                  IHubContext<StatusStartupHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _startupRepo = startupRepo;
            _docRepo = docRepo;
            _comStartupRepo = comStartupRepo;
            _contractRepo = contractRepo;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(StartupFormViewModel startup, IFormFile? logoUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");
                return View(startup);
            }

            // Kiểm tra nếu lĩnh vực mới được nhập
            LinhVuc linhVuc;
            if (!string.IsNullOrEmpty(startup.TenLinhVuc))
            {
                linhVuc = await _context.LinhVucs.FirstOrDefaultAsync(x => x.TenLinhVuc == startup.TenLinhVuc);

                if (linhVuc == null)
                {
                    linhVuc = new LinhVuc { TenLinhVuc = startup.TenLinhVuc };
                    _context.LinhVucs.Add(linhVuc);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                linhVuc = await _context.LinhVucs.FindAsync(startup.IDLinhVuc);
                if (linhVuc == null)
                {
                    ModelState.AddModelError("IDLinhVuc", "Lĩnh vực không hợp lệ.");
                    ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");
                    return View(startup);
                }
            }

            // Lưu ảnh nếu có
            string? logoPath = null;
            if (logoUrl != null)
            {
                try
                {
                    logoPath = await ImageUtil.SaveAsync(logoUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu tệp: " + ex.Message);
                 
                    return View(startup);
                }
            }

            // Tạo đối tượng Startup
            var newStartup = new Startup
            {
                TenStartup = startup.TenStartup,
                LogoUrl = logoPath,
                MoTa = startup.MoTa,
                IDLinhVuc = linhVuc.IDLinhVuc, // Gán ID lĩnh vực
                MucTieu = startup.MucTieu ?? 0,
                PhanTramCoPhan = startup.PhanTramCoPhan ?? 0,
                IDNguoiDung = (await _userManager.GetUserAsync(User))?.Id,
                NgayTao = DateTime.Now,
                TrangThaiXetDuyet = TrangThaiXetDuyetEnum.ChoDuyet
            };

            // Lưu vào DB
            _context.Startups.Add(newStartup);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("StartupID", newStartup.IDStartup);

            await _hubContext.Clients.All.SendAsync("ReceiveStartupAdd", "Thanh cong them" + newStartup.IDStartup.ToString());

            return RedirectToAction("Create", "GiayToes");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var startup = await _context.Startups
                .Include(s => s.GiayTos)
                .Include(s => s.DanhGiaStartups)
                .FirstOrDefaultAsync(s => s.IDStartup == id);

            if (startup == null)
                return Json(new { success = false, message = "Startup không tồn tại!" });

            _context.GiayTos.RemoveRange(startup.GiayTos);
            _context.DanhGiaStartups.RemoveRange(startup.DanhGiaStartups);
            _context.Startups.Remove(startup);

            try
            {
                TempData["Message"] = "Delete Success!";
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Delete Unsuccess!" + ex.Message;
            }

            await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", id, (int)startup.TrangThaiXetDuyet);
 
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? idStartup)
        {
            if (idStartup == null)
            {
                TempData["Message"] = "Don't have id to update";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var startup = await _context.Startups
                                 .Include(x => x.LinhVuc)
                                 .FirstOrDefaultAsync(x => x.IDStartup == idStartup);

            if (startup == null)
            {
                TempData["Message"] = "Can't find startup you want to update";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var user = _userManager.GetUserId(User);

            if (startup.IDNguoiDung != user)
            {
                TempData["Message"] = "You do not have permission to edit";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            StartupFormViewModel model = new StartupFormViewModel()
            {
                IDStartup = startup.IDStartup,
                TenStartup = startup.TenStartup,
                LogoUrl = startup.LogoUrl,
                MoTa = startup.MoTa,
                IDLinhVuc = startup.IDLinhVuc,
                TenLinhVuc = startup.LinhVuc?.TenLinhVuc,
                MucTieu = startup.MucTieu,
                PhanTramCoPhan = startup.PhanTramCoPhan
            };

            ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(StartupFormViewModel startup, IFormFile? logoUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");
                return View(startup);
            }

            // Lấy startup hiện tại từ DB
            var existingStartup = await _startupRepo.GetByIdAsync(startup.IDStartup);
            if (existingStartup == null) return NotFound();

            // Cập nhật thông tin
            existingStartup.TenStartup = startup.TenStartup;
            existingStartup.MoTa = startup.MoTa;
            existingStartup.IDLinhVuc = startup.IDLinhVuc;
            existingStartup.MucTieu = startup.MucTieu ?? 0;
            existingStartup.PhanTramCoPhan = startup.PhanTramCoPhan ?? 0;
            existingStartup.TrangThaiXetDuyet = TrangThaiXetDuyetEnum.ChoDuyet;

            if (logoUrl != null)
            {
                try
                {
                    existingStartup.LogoUrl = await ImageUtil.SaveAsync(logoUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu tệp: " + ex.Message);
                    return View(startup);
                }
            }

            // Cập nhật DB
            await _startupRepo.UpdateAsync(existingStartup);

            await _hubContext.Clients.All.SendAsync("ReceiveStatusUpdate", existingStartup.IDStartup, (int)existingStartup.TrangThaiXetDuyet);
            TempData["Message"] = "Update Success!";
            return RedirectToAction("Index", "Dashboard");
        }

    }
}
