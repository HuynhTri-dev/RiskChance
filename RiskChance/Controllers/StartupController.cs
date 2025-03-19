using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel;
using System.Drawing;
using RiskChance.Utils;
using RiskChance.Models.ViewModel.StartupViewModel;

namespace QuanLyStartup.Controllers
{
    public class StartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public StartupController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = new StartupPageViewModel();
            model.TopStartups = await _context.DanhGiaStartups
                                .GroupBy(s => s.IDStartup)
                                .Select(g => new
                                {
                                    IDStartup = g.Key,
                                    DiemTrungBinh = g.Average(s => s.DiemDanhGia)
                                })
                                .OrderByDescending(s => s.DiemTrungBinh)
                                .Take(4)
                                .Join(_context.Startups.Where(st => st.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet),
                                      dg => dg.IDStartup,
                                      st => st.IDStartup,
                                      (dg, st) => new TopStartupModelView
                                      {
                                          IDStartup = st.IDStartup,
                                          TenStartup = st.TenStartup,
                                          MoTa = st.MoTa,
                                          DiemTrungBinh = dg.DiemTrungBinh,
                                          LogoUrl = st.LogoUrl
                                      })
                                .ToListAsync();

            model.StartupList = await _context.Startups.Include(s => s.LinhVuc).Where(x => x.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet).ToListAsync();

            model.TopBusiness = await _context.Startups
                                             .GroupBy(s => s.IDLinhVuc)
                                             .Select(g => new
                                             {
                                                 IDLinhVuc = g.Key,
                                                 SoLuongStartup = g.Count()
                                             })
                                             .OrderByDescending(lv => lv.SoLuongStartup)
                                             .Take(5)
                                             .Join(_context.LinhVucs,
                                                   top => top.IDLinhVuc,
                                                   lv => lv.IDLinhVuc,
                                                   (top, lv) => new TopBusinessViewModel
                                                   {
                                                       NameBusiness = lv.TenLinhVuc,
                                                       NumberOfStartup = top.SoLuongStartup
                                                   })
                                             .ToListAsync();

            model.TopInvestors = await _context.HopDongDauTus
                                                 .GroupBy(x => x.IDNguoiDung) 
                                                 .Select(g => new
                                                 {
                                                     IDNguoiDung = g.Key,
                                                     TongSoTien = g.Sum(x => x.TongTien),
                                                     NumberOfContract = g.Count()
                                                 })
                                                 .OrderByDescending(x => x.TongSoTien) 
                                                 .Take(5) 
                                                 .Join(_context.NguoiDungs, 
                                                       top => top.IDNguoiDung,
                                                       nd => nd.Id,
                                                       (top, nd) => new TopInvestorViewModel
                                                       {
                                                           FullName = nd.HoTen,
                                                           Email = nd.Email,
                                                           Profit = top.TongSoTien,
                                                           NumberOfContract = top.NumberOfContract,
                                                       })
                                                 .ToListAsync();


            var user = await _userManager.GetUserAsync(User);
            ViewBag.User = user;
            ViewBag.ActivePage = "startup";

            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> LoadMore(int page = 1)
        //{
        //    var pageSize = 12;
        //    var startups = await _context.Startups
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize + 1) // Lấy thêm 1 để kiểm tra còn dữ liệu không
        //        .ToListAsync();

        //    bool hasMore = startups.Count > pageSize;
        //    if (hasMore) startups.RemoveAt(startups.Count - 1); // Xóa phần tử thừa

        //    return Json(new { startups, hasMore });
        //}

        // Add
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
                logoPath = await ImageUtil.SaveAsync(logoUrl);
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

        // Update
        // Delete
    }
}
