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

        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public TrangThaiXetDuyetEnum TrangThaiXetDuyet { get; set; } = TrangThaiXetDuyetEnum.ChoDuyet;

        public string? IDNguoiDung { get; set; }
        [ForeignKey("IDNguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        public ICollection<TinTucHashtag> TinTucHashtags { get; set; } = new List<TinTucHashtag>();
    }

    public enum TrangThaiXetDuyetEnum
    {
        ChoDuyet = 0,
        DaDuyet = 1,
        TuChoi = 2
    }
}
