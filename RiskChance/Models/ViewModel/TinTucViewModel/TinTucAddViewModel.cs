using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class TinTucAddViewModel
    {
        public string? ImgTinTuc { get; set; } = string.Empty;

        public string NoiDung { get; set; } = string.Empty;

        public List<string> Hashtags { get; set; } = new List<string>();
    }
}
