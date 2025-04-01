using RiskChance.Models;

namespace RiskChance.Areas.Investor.Models
{
    public class InvestorDashboardViewModel
    {
        public decimal TotalInvestment { get; set; }
        public decimal ExpectProfit { get; set; }
        public IEnumerable<HopDongDauTu>? HopDongDauTus { get; set; } = new List<HopDongDauTu>();
    }
}
