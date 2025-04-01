using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Areas.Founder.Controllers
{
    public class PayProfitController : Controller
    {
        private readonly IRepository<ThanhToanLoiNhuan> _payProfitRepo;
        private readonly ApplicationDBContext _context;

        public PayProfitController(IRepository<ThanhToanLoiNhuan> payProfitRepo,
            ApplicationDBContext context)
        {
            _payProfitRepo = payProfitRepo;
            _context = context; 
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
        public ActionResult Create(IFormCollection collection)
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
