using RiskChance.Models;

namespace RiskChance.Areas.Investor.Models
{
    public class InvestorDashboardViewModel
    {
        public decimal TotalInvestment { get; set; } = 0;
        public decimal ExpectProfit { get; set; } = 0;
        public decimal ProfitReceived { get; set; } = 0;
        public IEnumerable<HopDongDauTu>? HopDongDauTus { get; set; } = new List<HopDongDauTu>();
    }
}
