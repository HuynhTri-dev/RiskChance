using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyStartup.Models
{
    [Table("DanhGiaStartup")]
    public class DanhGiaStartup
    {
        [Key]
        public int ID { get; set; }
        public int DiemDanhGia { get; set; }
        public string? NhanXet { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Chỉ giữ lại ID, không có navigation properties
        public int IDStartup { get; set; }
        public string IDNguoiDung { get; set; }

    }
}
