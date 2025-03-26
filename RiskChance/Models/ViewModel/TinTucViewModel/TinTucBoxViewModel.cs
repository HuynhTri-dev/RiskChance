using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class TinTucBoxViewModel
    {
        [Required]
        public int IDTinTuc { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? ImgTinTuc { get; set; }
        public string? NoiDung { get; set; }
        public DateTime? NgayDang { get; set; }
        public string? IDNguoiDang { get; set; }
        public string NameNguoiDang { get; set; } = "Ẩn danh";
        public string ImgNguoiDang { get; set; } = "~/wwwroot/assests/user/image.png";
        public IEnumerable<Hashtag>? Hashtags = new List<Hashtag>();
    }
}
