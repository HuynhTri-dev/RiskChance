using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiskChance.Models
{
    [Table("Hashtag")]
    public class Hashtag
    {
        [Key]
        public int IDHashtag { get; set; }

        public string TenHashtag { get; set; } = string.Empty;
        public ICollection<TinTucHashtag> TinTucHashtags { get; set; } = new List<TinTucHashtag>();
    }
}
