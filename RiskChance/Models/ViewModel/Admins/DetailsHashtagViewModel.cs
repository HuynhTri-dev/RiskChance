namespace RiskChance.Models.ViewModel.Admins
{
    public class DetailsHashtagViewModel
    {
        public Hashtag hashtag { get; set; } = new Hashtag();
        public List<TinTuc>? TinTucs { get; set; }
    }
}
