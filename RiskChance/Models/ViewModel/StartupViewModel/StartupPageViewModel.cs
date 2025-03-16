using RiskChance.Models.ViewModel.StartupViewModel;
using RiskChance.Models;
namespace RiskChance.Models.ViewModel.StartupViewModel
{
    public class StartupPageViewModel
    {
        // Danh sách các nhà đầu tư được duyệt, sắp xếp theo tiêu chí nào tổng số tiền đã đầu tư
        public IEnumerable<TopInvestorViewModel> TopInvestors { get; set; } = new List<TopInvestorViewModel>();

        public IEnumerable<Startup> StartupList { get; set;} = new List<Startup>();

        public IEnumerable<TopStartupModelView> TopStartups { get; set; } = new List<TopStartupModelView>();

        public IEnumerable<TopBusinessViewModel> TopBusiness { get; set; } = new List<TopBusinessViewModel>();
    }
}
