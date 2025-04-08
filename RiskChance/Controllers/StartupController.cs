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
using RiskChance.Repositories;

namespace QuanLyStartup.Controllers
{
    [AllowAnonymous]
    public class StartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IRepository<Startup> _startupRepo;
        private readonly IRepository<GiayTo> _docRepo;
        private readonly IRepository<DanhGiaStartup> _comStartupRepo;
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private const int PageSize = 9;

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
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                }
            }

            var model = new StartupPageViewModel();
            model.TopStartups = await _context.DanhGiaStartups
                                .Where(s => s.DiemDanhGia != null)
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
                                          DiemTrungBinh = dg.DiemTrungBinh,
                                          LogoUrl = st.LogoUrl
                                      })
                                .ToListAsync();

            model.StartupList = await GetPagedAsync(1, PageSize);

            model.TopBusiness = await _context.Startups
                                             .Where(x => x.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
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
                                                 .Where(x => x.TrangThaiKyKet == TrangThaiKyKetEnum.DaDuyet && x.ThanhToan == true && x.TongTien > 0) 
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
                                                           Id = nd.Id,
                                                           FullName = nd.HoTen,
                                                           Email = nd.Email,
                                                           Profit = top.TongSoTien,
                                                           NumberOfContract = top.NumberOfContract,
                                                           AvatartUrl = nd.AvatarUrl,
                                                       })
                                                 .ToListAsync();

            ViewBag.ActivePage = "startup";

            return View(model);
        }

        public async Task<IEnumerable<Startup>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _context.Startups
                .Where(s => s.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet)
                .OrderByDescending(s => s.NgayTao)
                .Include(s => s.LinhVuc)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> LoadMore(int pageIndex = 1)
        {
            var startups = await GetPagedAsync(pageIndex, PageSize);
            return PartialView("_StartupListPartial", startups);
        }

        [HttpGet]
        public async Task<IActionResult> SearchStartups(string query)
        {
            var startups = await _context.Startups
                .Include(s => s.LinhVuc)
                .Where(s => s.TrangThaiXetDuyet == TrangThaiXetDuyetEnum.DaDuyet &&
                            (string.IsNullOrEmpty(query) ||
                             s.TenStartup.Contains(query) ||
                             (s.LinhVuc != null && s.LinhVuc.TenLinhVuc.Contains(query)))) // Tìm theo tên startup hoặc lĩnh vực
                .ToListAsync();

            return PartialView("_StartupListPartial", startups);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var startup = await _context.Startups
                .Include(x => x.NguoiDung)
                .Include(x => x.LinhVuc)
                .FirstOrDefaultAsync(x => x.IDStartup == id);

            if (startup == null)
            {
                return NotFound();
            }

            var doc = (await _docRepo.GetAllAsync())
                        .Where(x => x.IDStartup == startup.IDStartup)
                        .ToList();


            var amount = (await _contractRepo.GetAllAsync())
                        .Where(x => x.IDStartup == startup.IDStartup && x.TrangThaiKyKet == TrangThaiKyKetEnum.DaDuyet && x.ThanhToan == true)
                        .Sum(x => x.TongTien);


            DetailOfStartupViewModel model = new DetailOfStartupViewModel()
            {
                IDStartup = startup.IDStartup,
                LogoUrl = startup.LogoUrl,
                Business = startup.LinhVuc?.TenLinhVuc,
                Name = startup.TenStartup,
                Description = startup.MoTa,
                Target = startup.MucTieu,
                PercentOfCompany = startup.PhanTramCoPhan,
                AmountInvested = amount,
                DocumentList = doc,
                FounderId = startup.IDNguoiDung,
                FounderName = startup.NguoiDung?.HoTen
            };

            return View(model);
        }
    }
}
