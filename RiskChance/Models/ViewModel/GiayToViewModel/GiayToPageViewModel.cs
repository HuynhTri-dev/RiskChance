namespace RiskChance.Models.ViewModel.GiayToViewModel
{
    public class GiayToPageViewModel
    {
        public AddGiayToViewModel AddGiayToViewModel { get; set; } = new AddGiayToViewModel();
        public List<GiayTo> ListDocs { get; set; } = new List<GiayTo>();
    }
}
