using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;
using RiskChance.Models.ViewModel.GiayToViewModel;
using RiskChance.Utils;

namespace RiskChance.Controllers
{
    [AllowAnonymous]
    public class GiayToesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public GiayToesController(ApplicationDBContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GiayToes
        
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.GiayTos.Include(g => g.Startup);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: GiayToes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giayTo = await _context.GiayTos
                .Include(g => g.Startup)
                .FirstOrDefaultAsync(m => m.IDGiayTo == id);
            if (giayTo == null)
            {
                return NotFound();
            }

            return View(giayTo);
        }
    }
}
