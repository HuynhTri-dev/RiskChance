namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class TinTucPageViewModel
    {
        public IEnumerable<Hashtag> TopHashTag { get; set; } = new List<Hashtag>();
        public IEnumerable<TinTucBoxViewModel> NewsList { get; set; } = new List<TinTucBoxViewModel>();
        public IEnumerable<TinTucBoxViewModel> TopNews { get; set; } = new List<TinTucBoxViewModel>();
    }
}
