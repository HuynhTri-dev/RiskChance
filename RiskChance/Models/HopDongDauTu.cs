using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{

    [Table("HopDongDauTu")]
    public class HopDongDauTu
    {
        [Key]
        public int IDHopDong { get; set; }

        [Url(ErrorMessage = "Vui lòng nhập URL hợp lệ.")]
        public string? FileUrl { get; set; }

        [Required]
        public DateTime NgayKyKet { get; set; } = DateTime.Now;

        public decimal? TongTien { get; set; }

        public double? PhanTramLoiNhuan { get; set; }

        public string? NoiDung { get; set; }

        [Required]
        public TrangThaiKyKetEnum TrangThaiKyKet { get; set; } = TrangThaiKyKetEnum.DaGui;
        public bool? ThanhToan { get; set; }

        // Khóa ngoại
        public int? IDStartup { get; set; }
        [ForeignKey("IDStartup")]
        public Startup? Startup { get; set; }

        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }
        public ICollection<ThanhToanLoiNhuan>? ThanhToanLoiNhuans { get; set; } = new List<ThanhToanLoiNhuan>();
    }

    public enum TrangThaiKyKetEnum
    {
        DaGui = 0,    // Hợp đồng đã gửi cho đối tác và chờ duyệt
        DaDuyet = 1,   // Hợp đồng đã được duyệt và có hiệu lực
        BiTuChoi = 2   // Hợp đồng đã bị từ chối
    }
}
