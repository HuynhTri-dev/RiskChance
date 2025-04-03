using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    public class AccessLog
    {
        [Key]
        public int Id { get; set; }

        public string? IPAddress { get; set; }

        public DateTime AccessTime { get; set; }

        public string? UserAgent { get; set; }

        // UserId có thể null → nếu không đăng nhập hoặc là admin
        public string? UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public NguoiDung? User { get; set; }
    }
}
