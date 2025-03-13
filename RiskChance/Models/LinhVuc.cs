using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiskChance.Models
{
    [Table("LinhVuc")]
    public class LinhVuc
    {
        [Key]
        public int IDLinhVuc { get; set; }

        public string TenLinhVuc { get; set; } = string.Empty;

        // Quan hệ 1-N với Startup
        public ICollection<Startup> Startups { get; set; } = new List<Startup>();
    }
}
