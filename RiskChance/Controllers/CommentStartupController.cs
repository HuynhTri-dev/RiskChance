using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;
using System.ComponentModel;

namespace RiskChance.Controllers
{
    public class CommentStartupController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<DanhGiaStartup> _danhGiaRepo;

        public CommentStartupController(ApplicationDBContext context, IRepository<DanhGiaStartup> danhGiaRepo)
        {
            _context = context;
            _danhGiaRepo = danhGiaRepo;
        }

        public async Task<IActionResult> CommentEachStartup(int? id)
        {
            if (id == null)
                return NotFound();

            var danhGiaList = await _context.DanhGiaStartups
                .Include(x => x.NguoiDung)
                .Where(x => x.IDStartup == id)
                .OrderByDescending(x => x.NgayDanhGia)
                .ToListAsync();

            return View(danhGiaList);
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int idStartup)
        {
            var vc = HttpContext.RequestServices.GetService(typeof(IViewComponentHelper)) as IViewComponentHelper;
            if (vc == null) return BadRequest("Không tìm thấy ViewComponentHelper.");

            var viewContext = new ViewContext();
            (vc as IViewContextAware)?.Contextualize(viewContext);

            var result = await vc.InvokeAsync("CommentEachStartup", new { idStartup });

            return Content(result.ToString(), "text/html");
        }



    }
}
