using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RiskChance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.FeatureActive = "profile";
            return View();
        }
    }
}
