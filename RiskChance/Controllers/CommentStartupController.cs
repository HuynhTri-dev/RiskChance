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
        private readonly IViewComponentHelper _viewComponentHelper;

        public CommentStartupController(ApplicationDBContext context, IRepository<DanhGiaStartup> danhGiaRepo, IViewComponentHelper viewComponentHelper)
        {
            _context = context;
            _danhGiaRepo = danhGiaRepo;
            _viewComponentHelper = viewComponentHelper;
        }

        //public async Task<IActionResult> CommentEachStartup(int? id)
        //{
        //    if (id == null)
        //        return NotFound();

        //    var danhGiaList = await _context.DanhGiaStartups
        //        .Include(x => x.NguoiDung)
        //        .Where(x => x.IDStartup == id)
        //        .OrderByDescending(x => x.NgayDanhGia)
        //        .ToListAsync();

        //    return View(danhGiaList);
        //}

    }
}
