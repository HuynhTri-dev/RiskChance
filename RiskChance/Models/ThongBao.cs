using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("ThongBao")]
    public class ThongBao
    {
        [Key]
        public int IDNoti { get; set; }

        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayGui { get; set; } = DateTime.Now;

        // Thêm trạng thái thông báo (0: Chưa đọc, 1: Đã đọc)
        [Required]
        public TrangThaiThongBao TrangThai { get; set; } = TrangThaiThongBao.ChuaDoc;

        // Khóa ngoại
        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }
    }

    public enum TrangThaiThongBao
    {
        ChuaDoc = 0,
        DaDoc = 1
    }
}
