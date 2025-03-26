using Microsoft.AspNetCore.Mvc.Rendering;
using RiskChance.Models;

namespace RiskChance.Areas.Founder.ViewModels
{
    public class DashboardViewModel
    {
        public List<SelectListItem>? startupSelectList { get; set; }
        public int? SelectedStartupId { get; set; }
        public Startup? SelectStartup { get; set; }
        public int InteractView { get; set; } = 0;
        public int CoInvestors { get; set;} = 0;
        public decimal? TotalInvestment { get; set; }
    }
}
