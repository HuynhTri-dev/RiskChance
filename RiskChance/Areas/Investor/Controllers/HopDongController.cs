using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Authorize]
    public class HopDongController : Controller
    {
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private readonly IRepository<Startup> _startupRepo;
        private readonly UserManager<NguoiDung> _userManager;
        private readonly ApplicationDBContext _context;
        private readonly NotificationService _notificationService;

        public HopDongController(IRepository<HopDongDauTu> contract,
                                UserManager<NguoiDung> userManager,
                                IRepository<Startup> startupRepo,
                                ApplicationDBContext context,
                                NotificationService notificationService)
        {
            _contractRepo = contract;
            _userManager = userManager;
            _startupRepo = startupRepo;
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("HopDong/{idStartup:int}")]
        [Authorize(Roles = "Investor")]
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
        [Authorize(Roles = "Investor")]
        public async Task<IActionResult> Create(HopDongDauTu hopDong, IFormFile FileUrl)
        {
            if (!ModelState.IsValid)
                return View(hopDong);


            if (hopDong.IDStartup == null)
            {
                ModelState.AddModelError("", "Startup is not valid");
                return View(hopDong);
            }

            var investorId = HttpContext.Session.GetString("UserId");

            if (investorId == null)
            {
                ModelState.AddModelError("", "User is not valid");
                return View(hopDong);
            }

            var startup = await _startupRepo.GetByIdAsync(hopDong.IDStartup);
            if (startup == null)
            {
                ModelState.AddModelError("", "Startup is not valid");
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
                    ModelState.AddModelError("", "Error when save file: " + ex.Message);
                    return View(hopDong);
                }
            }

            hopDong.TrangThaiKyKet = TrangThaiKyKetEnum.DaGui;
            hopDong.NgayKyKet = DateTime.Now;
            hopDong.FileUrl = fileUrl;
            hopDong.IDNguoiDung = investorId;
            hopDong.ThanhToan = false;

            try
            {

                await _contractRepo.AddAsync(hopDong);

                // tao thanh cong thi minh thong bao
                var notif = new ThongBao
                {
                    IDNguoiGui = investorId,
                    NoiDung = $"You have a new contract from new investor",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = startup.IDNguoiDung
                };

                await _notificationService.SendNotification(notif);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unsuccess: " + ex.Message);
                return View(hopDong);
            }

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
                                        .ThenInclude(x => x.NguoiDung)
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

            try
            {
                await _contractRepo.UpdateAsync(contractExist);

                var userID = HttpContext.Session.GetString("UserId");
                // tao thanh cong thi minh thong bao
                var notif = new ThongBao
                {
                    IDNguoiGui = userID,
                    NoiDung = $"Your contract has been signed in. Please make the payment for your contract.",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = contractExist.IDNguoiDung
                };

                await _notificationService.SendNotification(notif);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Sign Unsuccess";

                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
            }

            TempData["Message"] = "Sign Success";

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

            try
            {
                await _contractRepo.UpdateAsync(contractExist);

                var userID = HttpContext.Session.GetString("UserId");
                // tao thanh cong thi minh thong bao
                var notif = new ThongBao
                {
                    IDNguoiGui = userID,
                    NoiDung = $"Your contract has been signed in. Please make the payment for your contract.",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = contractExist.IDNguoiDung
                };

                await _notificationService.SendNotification(notif);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Deny Unsuccess";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = id });
            }

            TempData["Message"] = "Deny Success";
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
            contractExist.PhanTramLoiNhuan = hopDong.PhanTramLoiNhuan;
            contractExist.NoiDung = hopDong.NoiDung;
            if (fileUrl != null)
            {
                contractExist.FileUrl = fileUrl;
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Successful Update";

            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
        }

        [HttpPost]
        [Authorize(Roles = "Investor")]
        public async Task<IActionResult> ThanhToan(HopDongDauTu hopDong, IFormFile? MinhChungThanhToan)
        {
            if (hopDong.IDHopDong == null) return NotFound();

            var existingContract = await _contractRepo.GetByIdAsync(hopDong.IDHopDong);

            if (existingContract == null)
            {
                TempData["Message"] = "Don't find your contract";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
            }

            string fileUrl = null;
            if (MinhChungThanhToan != null && MinhChungThanhToan.Length > 0)
            {
                try
                {
                    fileUrl = await ImageUtil.SaveAsync(MinhChungThanhToan);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Fail when update: " + ex.Message);
                    return View(hopDong);
                }
            }

            try
            {
                existingContract.ThanhToan = true;
                existingContract.MinhChungThanhToan = fileUrl;
                await _context.SaveChangesAsync();

                var userID = HttpContext.Session.GetString("UserId");
                var startup = await _startupRepo.GetByIdAsync(existingContract.IDStartup);
                // tao thanh cong thi minh thong bao
                var notif = new ThongBao
                {
                    IDNguoiGui = userID,
                    NoiDung = $"Your contract has been paid",
                    NgayGui = DateTime.Now,
                    IDNguoiNhan = startup.IDNguoiDung
                };

                await _notificationService.SendNotification(notif);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error payment";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
            }

            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = hopDong.IDHopDong });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractRepo.GetByIdAsync(id);

            if (contract == null) return NotFound();

            try
            {
                await _contractRepo.DeleteAsync(id);
                TempData["Message"] = "Delete Success";

                if (User.IsInRole("Founder"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Founder" });
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Investor" });
                }
            }
            catch
            {
                TempData["Message"] = "Delete Unsuccess";

                if (User.IsInRole("Founder"))
                {
                    return RedirectToAction("Details", "HopDong", new { area = "Founder", idContract = contract.IDHopDong });
                }
                else
                {
                    return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = contract.IDHopDong });
                }
            }
        }
    }
}
