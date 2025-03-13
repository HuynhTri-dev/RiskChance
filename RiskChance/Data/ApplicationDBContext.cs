using Microsoft.EntityFrameworkCore;
using QuanLyStartup.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RiskChance.Data
{
    public class ApplicationDBContext : IdentityDbContext<NguoiDung>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<Startup> Startups { get; set; }
        public DbSet<HopDongDauTu> HopDongDauTus { get; set; }
        public DbSet<DanhGiaStartup> DanhGiaStartups { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<TinTuc> TinTucs { get; set; }
        public DbSet<GiayTo> GiayTos { get; set; }
        public DbSet<LinhVuc> LinhVucs { get; set; }
        public DbSet<TinTucHashtag> TinTucHashtags { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HopDongDauTu>()
                .Property(h => h.TongTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Startup>()
                .Property(s => s.MucTieu)
                .HasColumnType("decimal(18,2)");

            // Cấu hình khóa ngoại cho IDStartup
            modelBuilder.Entity<DanhGiaStartup>()
                .HasOne<Startup>()  // Không dùng .HasOne(d => d.Startup)
                .WithMany()
                .HasForeignKey(d => d.IDStartup)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình khóa ngoại cho IDNguoiDung
            modelBuilder.Entity<DanhGiaStartup>()
                .HasOne<NguoiDung>()  // Không dùng .HasOne(d => d.NguoiDung)
                .WithMany()
                .HasForeignKey(d => d.IDNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HopDongDauTu>()
                .HasOne(h => h.Startup)
                .WithMany(s => s.HopDongDauTus)
                .HasForeignKey(h => h.IDStartup)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HopDongDauTu>()
                .HasOne(h => h.NguoiDung)
                .WithMany(n => n.HopDongDauTus)
                .HasForeignKey(h => h.IDNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GiayTo>()
                .HasOne(g => g.Startup)
                .WithMany(s => s.GiayTos)
                .HasForeignKey(g => g.IDStartup)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Startup>()
                .HasOne(s => s.LinhVuc)
                .WithMany(l => l.Startups)
                .HasForeignKey(s => s.IDLinhVuc)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TinTucHashtag>()
                .HasKey(tt => new { tt.IDTinTuc, tt.IDHashtag });

            modelBuilder.Entity<TinTucHashtag>()
                .HasOne(tt => tt.TinTuc)
                .WithMany(t => t.TinTucHashtags)
                .HasForeignKey(tt => tt.IDTinTuc);

            modelBuilder.Entity<TinTucHashtag>()
                .HasOne(tt => tt.Hashtag)
                .WithMany(h => h.TinTucHashtags)
                .HasForeignKey(tt => tt.IDHashtag);
        }
    }
}
