using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyStartup.Models
{
    [Table("TinTuc")]
    public class TinTuc
    {
        [Key]
        public int IDTinTuc { get; set; }

        public string? ImgTinTuc { get; set; } = string.Empty;

        [Required, MaxLength(int.MaxValue)]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public string IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        public ICollection<TinTucHashtag> TinTucHashtags { get; set; } = new List<TinTucHashtag>();
    }
}
