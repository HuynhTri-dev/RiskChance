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

        public StartupController(ApplicationDBContext context,
                                 UserManager<NguoiDung> userManager,
                                 IRepository<Startup> startupRepo,
                                 IRepository<GiayTo> docRepo,
                                 IRepository<DanhGiaStartup> comStartupRepo,
                                 IRepository<HopDongDauTu> contractRepo)
        {
            _context = context;
            _userManager = userManager;
            _startupRepo = startupRepo;
            _docRepo = docRepo;
            _comStartupRepo = comStartupRepo;
            _contractRepo = contractRepo;
        }

        [HttpGet]
        [Authorize(Roles = "Founder, Admin")]
        public async Task<IActionResult> Add()
        {
            ViewBag.LinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "IDLinhVuc", "TenLinhVuc");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Founder, Admin")]
      
        public async Task<IActionResult> Add(StartupFormViewModel startup, IFormFile logoUrl)
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

            return RedirectToAction("Create", "GiayToes");
        }

    }
}
