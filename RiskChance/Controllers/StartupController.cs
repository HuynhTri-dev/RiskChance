using Microsoft.AspNetCore.Mvc;

namespace QuanLyStartup.Controllers
{
    public class StartupController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ActivePage = "startup";
            return View();
        }
    }
}
