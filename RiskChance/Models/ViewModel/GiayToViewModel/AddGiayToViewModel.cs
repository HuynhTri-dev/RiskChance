
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.GiayToViewModel
{
    public class AddGiayToViewModel
    {
        public int? IdDoc { get; set; }
        public string? NameDoc { get; set; }
        public string? TypeDoc { get; set; }
        public string? ContentDoc { get; set; }
        public string? FileUrl { get; set; }

        public IFormFile? GetFile { get; set;
        }
    }
}
