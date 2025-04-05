namespace RiskChance.Models.ViewModel.StartupViewModel
{
    public class DetailOfStartupViewModel
    {
        public int IDStartup { get; set; }
        public string? FounderId { get; set; }
        public string? Business { get; set; }
        public string? LogoUrl { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Target { get; set; }
        public double? PercentOfCompany { get; set; }
        public decimal? AmountInvested { get; set; }
        public List<GiayTo> DocumentList { get; set; } = new List<GiayTo>();
       
    }
}
