﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RiskChance.Models
{
    [Table("GiayTo")]
    public class GiayTo
    {
        [Key]
        public int IDGiayTo { get; set; }

        public string? FileGiayTo { get; set; }
        public string? LoaiFile { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string? TenGiayTo { get; set; }
        public int? IDStartup { get; set; }
        [ForeignKey("IDStartup")]
        public Startup? Startup { get; set; }
    }
}
