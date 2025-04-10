using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.StartupViewModel
{
    public class StartupFormViewModel
    {
        public int IDStartup { get; set; }

        [Required(ErrorMessage = "Name startup cannot be empty")]
        public string TenStartup { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot be length than 1000 letter")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Please choose startup's bussiness")]
        public int IDLinhVuc { get; set; }
        public string? TenLinhVuc { get; set; }

        [Required(ErrorMessage = "Target cannot be empty")]
        [Range(1000000, 1000000000000000,ErrorMessage = "Your target need more than 1.000.000 VND")]
        public decimal? MucTieu { get; set; }

        public double? PhanTramCoPhan { get; set; }
    }
}
