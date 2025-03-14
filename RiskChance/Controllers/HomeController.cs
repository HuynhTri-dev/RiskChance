using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;

namespace QuanLyStartup.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDBContext _context;

    public HomeController(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var topHashtags = await _context.Hashtags
            .Select(h => new
            {
                Name = h.TenHashtag,
                Count = h.TinTucHashtags.Count()
            })
            .OrderByDescending(h => h.Count)
            .Take(5)
            .ToListAsync();

        ViewBag.TopHashtags = topHashtags;
        ViewBag.ActivePage = "home";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
