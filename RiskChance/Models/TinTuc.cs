using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("TinTuc")]
    public class TinTuc
    {
        [Key]
        public int IDTinTuc { get; set; }

        public string? ImgTinTuc { get; set; } = string.Empty;
        [Required]
        public string TieuDe { get;set; } = string.Empty;
        [Required]
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayDang { get; set; } = DateTime.Now;
        [Required]
        public TrangThaiXetDuyetEnum TrangThaiXetDuyet { get; set; } = TrangThaiXetDuyetEnum.ChoDuyet;
        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        public ICollection<TinTucHashtag> TinTucHashtags { get; set; } = new List<TinTucHashtag>();

        public ICollection<BinhLuanTinTuc> BinhLuanTinTucs { get; set; } = new List<BinhLuanTinTuc>();

    }

    public enum TrangThaiXetDuyetEnum
    {
        ChoDuyet = 0,
        DaDuyet = 1,
        TuChoi = 2
    }
}
