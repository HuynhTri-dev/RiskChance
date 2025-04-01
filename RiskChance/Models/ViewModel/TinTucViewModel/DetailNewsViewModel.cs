namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class DetailNewsViewModel
    {
        public TinTuc TinTuc { get; set; } = new TinTuc();
        public BinhLuanTinTuc BinhLuanTinTuc { get; set; } = new BinhLuanTinTuc();
        public List<TinTuc> TinTucLienQuan { get; set; } = new List<TinTuc>();
        
    }
}
