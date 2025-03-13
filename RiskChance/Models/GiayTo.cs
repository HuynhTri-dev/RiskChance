using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyStartup.Models
{
    [Table("GiayTo")]
    public class GiayTo
    {
        [Key]
        public int IDGiayTo { get; set; }

        public string FileGiayTo { get; set; } = string.Empty;
        public string LoaiFile { get; set; } = string.Empty;
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string? TenGiayTo { get; set; }
        public int IDStartup { get; set; }
        [ForeignKey("IDStartup")]
        public Startup? Startup { get; set; }
    }
}
