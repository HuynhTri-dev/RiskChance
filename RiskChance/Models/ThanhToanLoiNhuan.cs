using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    public class ThanhToanLoiNhuan
    {
        [Key]
        public int IdThanhToan { get; set; }
        public int? IDHopDong { get; set; }
        [ForeignKey("IDHopDong")]
        public HopDongDauTu? HopDongDauTu { get; set; }
        [Required]
        public DateTime NgayThanhToan { get; set; }
        [Url]
        public string? MinhChungFile { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; } 
    }
}
