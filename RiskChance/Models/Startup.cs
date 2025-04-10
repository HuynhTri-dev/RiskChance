using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("Startup")]
    public class Startup
    {
        [Key]
        public int IDStartup { get; set; }

        [Required(ErrorMessage = "Name startup cannot be empty")]
        public string TenStartup { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public string? MoTa { get; set; }

        [Required]
        public int IDLinhVuc { get; set; }

        [ForeignKey("IDLinhVuc")]
        public LinhVuc? LinhVuc { get; set; }

        [Required]
        public decimal MucTieu { get; set; }
        public double? PhanTramCoPhan { get; set; } 

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        public TrangThaiXetDuyetEnum TrangThaiXetDuyet { get; set; } = TrangThaiXetDuyetEnum.ChoDuyet;

        // IDNguoiDung là khóa ngoại, bắt buộc
        [Required]
        public string? IDNguoiDung { get; set; }

        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        // Quan hệ 1 - N với GiayTo
        public ICollection<GiayTo> GiayTos { get; set; } = new List<GiayTo>();
        public ICollection<HopDongDauTu> HopDongDauTus { get; set; } = new List<HopDongDauTu>();
        public ICollection<DanhGiaStartup> DanhGiaStartups { get; set; } = new List<DanhGiaStartup>();
    }
}
