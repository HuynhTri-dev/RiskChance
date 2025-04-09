using RiskChance.Models.ViewModel.TinTucViewModel;

namespace RiskChance.Models.ViewModel.HomeViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Startup> StartupList { get; set; } = new List<Startup>();
        public IEnumerable<TinTucBoxViewModel> NewsList { get; set; } = new List<TinTucBoxViewModel>();
        public IEnumerable<Hashtag> TopHashTag { get; set; } = new List<Hashtag>();
    }
}
