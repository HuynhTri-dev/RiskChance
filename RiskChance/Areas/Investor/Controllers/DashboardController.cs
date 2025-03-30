using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RiskChance.Areas.Investor.Controllers
{
    [Area("Investor")]
    [Authorize(Roles = "Investor")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ActiveFeature = "dashboardInvestor";
            return View();
        }
    }
}
