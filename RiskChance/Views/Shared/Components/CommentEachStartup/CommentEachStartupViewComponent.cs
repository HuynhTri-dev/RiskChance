using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using System;

namespace RiskChance.Views.Shared.Components.CommentEachStartup
{
    [AllowAnonymous]
    public class CommentEachStartupViewComponent : ViewComponent
    {
        private readonly ApplicationDBContext _context;

        public CommentEachStartupViewComponent(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int startupId)
        {
            var comments = await _context.DanhGiaStartups
                .Include(x => x.NguoiDung)
                .Where(x => x.IDStartup == startupId)
                .OrderByDescending(x => x.NgayDanhGia)
                .ToListAsync();

            return View("CommentStartupList", comments);
        }
    }
}
