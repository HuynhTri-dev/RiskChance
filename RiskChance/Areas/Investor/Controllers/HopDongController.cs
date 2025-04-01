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

namespace RiskChance.Areas.Investor.Controllers
{
    [Area("Investor")]
    [Authorize(Roles="Investor, Founder")]
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

            var investorId = HttpContext.Session.GetString("UserId");

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

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> SignConfirm(HopDongDauTu hopDong, IFormFile FileUrl)
        {
            if (!ModelState.IsValid)
                return View(hopDong);

            var contractExist = await _contractRepo.GetByIdAsync(hopDong.IDHopDong);

            if (contractExist == null)
            {
                ModelState.AddModelError("", "None contract available");
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
                    ModelState.AddModelError("", "Fail when update: " + ex.Message);
                    return View(hopDong);
                }
            }

            contractExist.TrangThaiKyKet = TrangThaiKyKetEnum.DaDuyet;
            contractExist.NgayKyKet = DateTime.Now;
            contractExist.FileUrl = fileUrl;

            await _contractRepo.UpdateAsync(contractExist);

            TempData["Message"] = "Successful Sign";

            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
        }

        [HttpPost]
        public async Task<IActionResult> DenyContract(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Error deny contract";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = id });
            }

            var contractExist = await _contractRepo.GetByIdAsync(id);

            if (contractExist == null)
            {
                TempData["Message"] = "Error deny contract";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = id });
            }

            contractExist.TrangThaiKyKet = TrangThaiKyKetEnum.BiTuChoi;
            contractExist.NgayKyKet = DateTime.Now;

            await _contractRepo.UpdateAsync(contractExist);

            TempData["Message"] = "Successful Deny";
            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _contractRepo.GetByIdAsync(id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(HopDongDauTu hopDong, IFormFile? FileUrl)
        {
            if (!ModelState.IsValid)
                return View(hopDong);

            var contractExist = await _contractRepo.GetByIdAsync(hopDong.IDHopDong);

            if (contractExist == null)
            {
                ModelState.AddModelError("", "None contract available");
                return View(hopDong);
            }

            string? fileUrl = null;
            if (FileUrl != null && FileUrl.Length > 0)
            {
                try
                {
                    fileUrl = await DocumentUtil.SaveAsync(FileUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Fail when update: " + ex.Message);
                    return View(hopDong);
                }
            }

            contractExist.TrangThaiKyKet = TrangThaiKyKetEnum.DaGui;
            contractExist.NgayKyKet = DateTime.Now;
            if (fileUrl != null)
            {
                contractExist.FileUrl = fileUrl;
            }

            await _contractRepo.UpdateAsync(contractExist);

            TempData["Message"] = "Successful Update";

            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
        }
    }
}
