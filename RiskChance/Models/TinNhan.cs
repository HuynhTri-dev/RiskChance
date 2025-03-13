using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("TinNhan")]
    public class TinNhan
    {
        [Key]
        public int IDTinNhan { get; set; }
        public string NoiDung { get; set; } = string.Empty;

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [Required, MaxLength(50)]
        public string TrangThai { get; set; } = "Chưa đọc";

        // Khóa ngoại
        public string? IDNguoiGui { get; set; }
        [ForeignKey("IDNguoiGui")]
        public NguoiDung? NguoiGui { get; set; }

        public string? IDNguoiNhan { get; set; }
        [ForeignKey("IDNguoiNhan")]
        public NguoiDung? NguoiNhan { get; set; }
    }
}
