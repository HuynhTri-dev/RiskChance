using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models.ViewModel.TinTucViewModel
{
    public class TinTucAddViewModel
    {
        public int? IDTinTuc { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống!")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự!")]
        public string TieuDe { get; set; } = string.Empty;

        public string? ImgTinTuc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống!")]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime? NgayDang { get; set; } = DateTime.Now;

        public TrangThaiXetDuyetEnum trangThaiXetDuyet { get; set; } = TrangThaiXetDuyetEnum.ChoDuyet;

        [Required(ErrorMessage = "ID người dùng không được để trống!")]
        public string IDNguoiDung { get; set; } = string.Empty;

        public List<string>? Hashtags { get; set; } = new List<string>();
    }
}
