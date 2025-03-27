using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;
using RiskChance.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class HopDongController : Controller
    {
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private readonly IRepository<Startup> _startupRepo;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly ApplicationDBContext _context;

        public HopDongController(IRepository<HopDongDauTu> contract,
                                UserManager<NguoiDung> userManager,
                                IRepository<Startup> startupRepo,
                                ApplicationDBContext context)
        {
            _contractRepo = contract;
            _userManager = userManager;
            _startupRepo = startupRepo;
            _context = context;
        }

        [HttpGet]
        [Route("HopDong/{idStartup:int}")]
        public async Task<IActionResult> Create(int? idStartup)
        {
            if (idStartup == null)
                return NotFound();

            var startup = await _startupRepo.GetByIdAsync(idStartup);

            if (startup == null)
                return NotFound();

            ViewBag.NameStartup = startup.TenStartup;
            ViewBag.IdStartup = startup.IDStartup;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HopDongDauTu hopDong, IFormFile FileUrl)
        {
            if (!ModelState.IsValid)
                return View(hopDong);

            
            if (hopDong.IDStartup == null)
            {
                ModelState.AddModelError("", "Không có startup hợp lệ.");
                return View(hopDong);
            }

            var investorId = _userManager.GetUserId(User);

            if (investorId == null)
            {
                ModelState.AddModelError("", "Người dùng không hợp lệ.");
                return View(hopDong);
            }

            var startup = await _startupRepo.GetByIdAsync(hopDong.IDStartup);
            if (startup == null)
            {
                ModelState.AddModelError("", "Không có startup hợp lệ.");
                return View(hopDong);
            }

            string fileUrl = null;
            if (FileUrl != null && FileUrl.Length > 0)
            {
                try
                {
                    fileUrl = await DocumentUtil.SaveAsync(FileUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu tệp: " + ex.Message);
                    return View(hopDong);
                }
            }

            hopDong.TrangThaiKyKet = TrangThaiKyKetEnum.DaGui;
            hopDong.NgayKyKet = DateTime.Now;
            hopDong.FileUrl = fileUrl;
            hopDong.IDNguoiDung = investorId;

            await _contractRepo.AddAsync(hopDong);

            return RedirectToAction("Index", "Startup", new { area = "" });
        }

        public async Task<IActionResult> Details(int? idContract)
        {
            if (idContract == null)
                return NotFound();

            var contract = await _context.HopDongDauTus
                                    .Include(x => x.NguoiDung)
                                    .Include(x => x.Startup)
                                    .FirstOrDefaultAsync(x => x.IDHopDong == idContract);
                                   
            if (contract == null)
                return NotFound();

            return View(contract);
        }
    }
}
