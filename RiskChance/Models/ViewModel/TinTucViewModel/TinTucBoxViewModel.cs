using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class TinTucBoxViewModel
    {
        public int? IdTinTuc {  get; set; }
        public string? ImgTinTuc { get; set; }
        [Required]
        public string NoiDung { get; set; } = string.Empty;
        public DateTime NgayDang { get; set; }
        [Required]
        public string IDNguoiDang { get; set; } = string.Empty;
        [Required]
        public string NameNguoiDang { get; set; } = "Ẩn danh";
    }
}
