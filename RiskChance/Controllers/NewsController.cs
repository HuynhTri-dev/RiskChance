using Microsoft.AspNetCore.Mvc;

namespace QuanLyStartup.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ActivePage = "news";
            return View();
        }
    }
}
