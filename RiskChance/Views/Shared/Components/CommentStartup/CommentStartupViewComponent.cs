using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Repositories;

namespace RiskChance.Views.Shared.Components.CommentStartup
{
    public class CommentStartupViewComponent : ViewComponent
    {
        private readonly ApplicationDBContext _context;
        private readonly IRepository<DanhGiaStartup> _commentRepo;
        private readonly UserManager<NguoiDung> _userManager;

        public CommentStartupViewComponent(ApplicationDBContext context, IRepository<DanhGiaStartup> commentRepo, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        // Phương thức hiển thị form bình luận
        public async Task<IViewComponentResult> InvokeAsync(int startupId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var model = new DanhGiaStartup
            {
                IDStartup = startupId,
                IDNguoiDung = user?.Id,
            };

            return View("CommentStartup", model);
        }
    }
}
