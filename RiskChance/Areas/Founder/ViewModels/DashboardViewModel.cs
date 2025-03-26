using Microsoft.AspNetCore.Mvc.Rendering;
using RiskChance.Models;

namespace RiskChance.Areas.Founder.ViewModels
{
    public class DashboardViewModel
    {
        public int? SelectedStartupId { get; set; }
        public List<SelectListItem>? startupSelectList { get; set; }
    }
}
