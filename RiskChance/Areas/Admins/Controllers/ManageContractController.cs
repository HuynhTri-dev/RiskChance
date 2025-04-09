using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskChance.Data;
using RiskChance.Models;

namespace RiskChance.Areas.Admins.Controllers
{
    [Area("Admins")]
    [Authorize(Roles = "Admin")]
    public class ManageContractController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ManageContractController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveFeature = "manageContract";
            var contracts = await _context.HopDongDauTus.Include(x => x.Startup).Include(x => x.NguoiDung).ToListAsync();
            return View(contracts);
        }

        [HttpPost]
        public IActionResult ToggleTrangThaiKyKet(int id)
        {
            var hopDong = _context.HopDongDauTus.Find(id);
            if (hopDong == null) return NotFound();

            // Vòng tròn trạng thái: Đã gửi -> Đã duyệt -> Bị từ chối -> Đã gửi
            hopDong.TrangThaiKyKet = hopDong.TrangThaiKyKet switch
            {
                TrangThaiKyKetEnum.DaGui => TrangThaiKyKetEnum.DaDuyet,
                TrangThaiKyKetEnum.DaDuyet => TrangThaiKyKetEnum.BiTuChoi,
                TrangThaiKyKetEnum.BiTuChoi => TrangThaiKyKetEnum.DaGui,
                _ => TrangThaiKyKetEnum.DaGui
            };

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleThanhToan(int id)
        {
            var hopDong = _context.HopDongDauTus.Find(id);
            if (hopDong == null) return NotFound();

            hopDong.ThanhToan = !(hopDong.ThanhToan ?? false);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHopDong(int id)
        {
            try
            {
                var hopDong = _context.HopDongDauTus
                    .Include(h => h.ThanhToanLoiNhuans)
                    .FirstOrDefault(h => h.IDHopDong == id);

                if (hopDong == null)
                    return RedirectToAction("Index");

                // Xóa các thanh toán lợi nhuận liên quan
                _context.ThanhToanLoiNhuans.RemoveRange(hopDong.ThanhToanLoiNhuans);

                // Xóa chính hợp đồng
                _context.HopDongDauTus.Remove(hopDong);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }



    }
}
