using Microsoft.EntityFrameworkCore;
using RiskChance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
        public DbSet<BinhLuanTinTuc> BinhLuanTinTucs { get; set; }
        public DbSet<ThanhToanLoiNhuan> ThanhToanLoiNhuans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình kiểu dữ liệu Decimal
            modelBuilder.Entity<HopDongDauTu>()
                .Property(h => h.TongTien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Startup>()
                .Property(s => s.MucTieu)
                .HasColumnType("decimal(18,2)");

            // Khi xóa người dùng, giữ lại tin tức, tin nhắn, thông báo, bình luận (SetNull)
            modelBuilder.Entity<TinTuc>()
                .HasOne(tt => tt.NguoiDung)
                .WithMany(nd => nd.TinTucs)
                .HasForeignKey(tt => tt.IDNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<BinhLuanTinTuc>()
                .HasOne(bl => bl.NguoiDung)
                .WithMany(nd => nd.BinhLuanTinTucs)
                .HasForeignKey(bl => bl.IDNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);

            // Khi xóa người dùng, xóa luôn startup và giấy tờ liên quan (Cascade)
            modelBuilder.Entity<Startup>()
                .HasOne(s => s.NguoiDung)
                .WithMany(n => n.Startups)
                .HasForeignKey(s => s.IDNguoiDung)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GiayTo>()
                .HasOne(g => g.Startup)
                .WithMany(s => s.GiayTos)
                .HasForeignKey(g => g.IDStartup)
                .OnDelete(DeleteBehavior.Restrict);

            // Hợp đồng đầu tư: Khi xóa tài khoản thì giữ lại hợp đồng nhưng ID sẽ là NULL
            modelBuilder.Entity<HopDongDauTu>()
                .HasOne(h => h.Startup)
                .WithMany(s => s.HopDongDauTus)
                .HasForeignKey(h => h.IDStartup)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<HopDongDauTu>()
                .HasOne(h => h.NguoiDung)
                .WithMany(n => n.HopDongDauTus)
                .HasForeignKey(h => h.IDNguoiDung)
                .OnDelete(DeleteBehavior.SetNull);

            // Lĩnh vực: Khi xóa lĩnh vực thì giữ startup nhưng ID lĩnh vực = NULL
            modelBuilder.Entity<Startup>()
                .HasOne(s => s.LinhVuc)
                .WithMany(l => l.Startups)
                .HasForeignKey(s => s.IDLinhVuc)
                .OnDelete(DeleteBehavior.SetNull);

            // Liên kết TinTuc và Hashtag
            modelBuilder.Entity<TinTucHashtag>()
                .HasKey(tt => new { tt.IDTinTuc, tt.IDHashtag });

            modelBuilder.Entity<TinTucHashtag>()
                .HasOne(tt => tt.TinTuc)
                .WithMany(t => t.TinTucHashtags)
                .HasForeignKey(tt => tt.IDTinTuc)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TinTucHashtag>()
                .HasOne(tt => tt.Hashtag)
                .WithMany(h => h.TinTucHashtags)
                .HasForeignKey(tt => tt.IDHashtag)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<DanhGiaStartup>()
                .HasOne(dg => dg.Startup)
                .WithMany(s => s.DanhGiaStartups)
                .HasForeignKey(dg => dg.IDStartup)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThanhToanLoiNhuan>()
                .HasOne<HopDongDauTu>()
                .WithMany()
                .HasForeignKey(p => p.IDHopDong)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
