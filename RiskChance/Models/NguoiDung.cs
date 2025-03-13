using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyStartup.Models
{
    [Table("NguoiDung")]
    public class NguoiDung : IdentityUser
    {
        public string HoTen { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = "~/assets/user/image.png"; // Ảnh mặc định

        public string VaiTro { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Quan hệ 1-N
        public ICollection<Startup> Startups { get; set; } = new List<Startup>();
        public ICollection<HopDongDauTu> HopDongDauTus { get; set; } = new List<HopDongDauTu>();
    }
}
