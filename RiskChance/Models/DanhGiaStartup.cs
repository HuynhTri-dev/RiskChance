using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("DanhGiaStartup")]
    public class DanhGiaStartup
    {
        [Key]
        public int IDDanhGia { get; set; }
        public float DiemDanhGia { get; set; }
        public string? NhanXet { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Chỉ giữ lại ID, không có navigation properties
        [Required]
        public int IDStartup { get; set; }
        [ForeignKey("IDStartup")]
        public Startup? Startup { get; set; }

        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

    }
}
