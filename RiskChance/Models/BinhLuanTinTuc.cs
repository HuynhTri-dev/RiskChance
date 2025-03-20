using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiskChance.Models
{
    [Table("BinhLuanTinTuc")]
    public class BinhLuanTinTuc
    {
        [Key]
        public int IDBinhLuan { get; set; }
        [Required]
        public float DiemDanhGia { get; set; }
        public string? NhanXet { get; set; }
        [Required]
        public DateTime NgayBinhLuan { get; set; } = DateTime.Now;

        // Khóa ngoại đến bảng TinTuc
        [Required]
        public int IDTinTuc { get; set; }
        [ForeignKey("IDTinTuc")]
        public TinTuc? TinTuc { get; set; }

        // Khóa ngoại đến bảng NguoiDung
        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }
    }
}
