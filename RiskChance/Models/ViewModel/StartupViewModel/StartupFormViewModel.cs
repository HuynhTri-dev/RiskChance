using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.StartupViewModel
{
    public class StartupFormViewModel
    {
        public int IDStartup { get; set; }

        [Required(ErrorMessage = "Tên startup không được để trống")]
        public string TenStartup { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lĩnh vực")]
        public int IDLinhVuc { get; set; }
        public string? TenLinhVuc { get; set; }

        [Required(ErrorMessage = "Mục tiêu không được để trống")]
        [Range(1000000, 1000000000000000,ErrorMessage = "Your target need more than 1.000.000 VND")]
        public decimal? MucTieu { get; set; }

        public double? PhanTramCoPhan { get; set; }
    }
}
