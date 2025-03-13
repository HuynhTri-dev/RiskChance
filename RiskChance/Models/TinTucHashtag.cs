using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyStartup.Models
{
    [Table("TinTucHashtag")]
    public class TinTucHashtag
    {
        // Khóa ngoại đến TinTuc
        public int IDTinTuc { get; set; }
        [ForeignKey("IDTinTuc")]
        public TinTuc? TinTuc { get; set; }

        // Khóa ngoại đến Hashtag
        public int IDHashtag { get; set; }
        [ForeignKey("IDHashtag")]
        public Hashtag? Hashtag { get; set; }
    }
}
