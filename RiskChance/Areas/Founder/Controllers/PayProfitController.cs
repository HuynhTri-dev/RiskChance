using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;
using RiskChance.Utils;
using System.Diagnostics.Contracts;

namespace RiskChance.Areas.Founder.Controllers
{
    [Area("Founder")]
    [Authorize(Roles ="Founder")]
    public class PayProfitController : Controller
    {
        private readonly IRepository<ThanhToanLoiNhuan> _payProfitRepo;
        private readonly IRepository<HopDongDauTu> _contractRepo;
        private readonly NotificationService _notificationService;
        private readonly ApplicationDBContext _context;

        public PayProfitController(IRepository<ThanhToanLoiNhuan> payProfitRepo,
            ApplicationDBContext context,
            IRepository<HopDongDauTu> contractRepo,
            NotificationService notificationService
            )
        {
            _payProfitRepo = payProfitRepo;
            _context = context; 
            _contractRepo = contractRepo;
            _notificationService = notificationService;
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhToanLoiNhuan model, IFormFile MinhChungFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Have error when paid";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = model.IDHopDong });
            }

            var contract = await _contractRepo.GetByIdAsync(model.IDHopDong);

            if (contract == null)
            {
                TempData["Message"] = "Cannot find your contract";
                return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = model.IDHopDong });
            }
        

            if (MinhChungFile != null && MinhChungFile.Length > 0)
            {
                try
                {
                    model.MinhChungFile = await ImageUtil.SaveAsync(MinhChungFile);
                }
                catch
                {
                    TempData["Message"] = "Error when save your paid check";
                    return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = model.IDHopDong });
                }
            }

            await _payProfitRepo.AddAsync(model);

            var userID = HttpContext.Session.GetString("UserId");
            // tao thanh cong thi minh thong bao
            var notif = new ThongBao
            {
                IDNguoiGui = userID,
                NoiDung = $"You has been get the profit",
                NgayGui = DateTime.Now,
                IDNguoiNhan = contract.IDNguoiDung
            };

            await _notificationService.SendNotification(notif);

            TempData["Message"] = "Payment success";
            return RedirectToAction("Details", "HopDong", new { area = "Investor", idContract = model.IDHopDong });
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PayProfitControlller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PayProfitControlller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PayProfitControlller/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
