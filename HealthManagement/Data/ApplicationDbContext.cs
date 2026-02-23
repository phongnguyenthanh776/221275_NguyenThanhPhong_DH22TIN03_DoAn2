using HealthManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Data
{
    /// <summary>
    /// DbContext chính của ứng dụng - quản lý các bảng cốt lõi
    /// Kế thừa từ IdentityDbContext để sử dụng ASP.NET Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho các bảng đang sử dụng
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<VaiTro> VaiTro { get; set; }
        public DbSet<HoSoSucKhoe> HoSoSucKhoe { get; set; }
        public DbSet<ChiSoSucKhoe> ChiSoSucKhoe { get; set; }
        public DbSet<LichSuBMI> LichSuBMI { get; set; }
        public DbSet<DuDoanAI> DuDoanAI { get; set; }
        public DbSet<GiacNgu> GiacNgu { get; set; }
        public DbSet<UongNuoc> UongNuoc { get; set; }
        public DbSet<Thuoc> Thuoc { get; set; }
        public DbSet<LichUongThuoc> LichUongThuoc { get; set; }
        public DbSet<NhacUongNuoc> NhacUongNuoc { get; set; }
        public DbSet<TinTucSucKhoe> TinTucSucKhoe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các quan hệ Foreign Key và ràng buộc

            // NguoiDung - VaiTro (1-n)
            modelBuilder.Entity<NguoiDung>()
                .HasOne(nd => nd.VaiTro)
                .WithMany(vt => vt.NguoiDung)
                .HasForeignKey(nd => nd.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            // NguoiDung - HoSoSucKhoe (1-1)
            modelBuilder.Entity<HoSoSucKhoe>()
                .HasOne(hs => hs.NguoiDung)
                .WithOne(nd => nd.HoSoSucKhoe)
                .HasForeignKey<HoSoSucKhoe>(hs => hs.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - ChiSoSucKhoe (1-n)
            modelBuilder.Entity<ChiSoSucKhoe>()
                .HasOne(cs => cs.NguoiDung)
                .WithMany(nd => nd.ChiSoSucKhoe)
                .HasForeignKey(cs => cs.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - LichSuBMI (1-n)
            modelBuilder.Entity<LichSuBMI>()
                .HasOne(ls => ls.NguoiDung)
                .WithMany(nd => nd.LichSuBMI)
                .HasForeignKey(ls => ls.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - DuDoanAI (1-n)
            modelBuilder.Entity<DuDoanAI>()
                .HasOne(dd => dd.NguoiDung)
                .WithMany(nd => nd.DuDoanAI)
                .HasForeignKey(dd => dd.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // Lifestyle: GiacNgu (1-n)
            modelBuilder.Entity<GiacNgu>()
                .HasOne(gn => gn.NguoiDung)
                .WithMany(nd => nd.GiacNgu)
                .HasForeignKey(gn => gn.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // Lifestyle: UongNuoc (1-n)
            modelBuilder.Entity<UongNuoc>()
                .HasOne(un => un.NguoiDung)
                .WithMany(nd => nd.UongNuoc)
                .HasForeignKey(un => un.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // Water reminders (1-n)
            modelBuilder.Entity<NhacUongNuoc>()
                .HasOne(w => w.NguoiDung)
                .WithMany(nd => nd.NhacUongNuoc)
                .HasForeignKey(w => w.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // Thuoc (1-n) NguoiDung
            modelBuilder.Entity<Thuoc>()
                .HasOne(t => t.NguoiDung)
                .WithMany(nd => nd.Thuoc)
                .HasForeignKey(t => t.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // LichUongThuoc (1-n) Thuoc
            modelBuilder.Entity<LichUongThuoc>()
                .HasOne(l => l.Thuoc)
                .WithMany(t => t.LichUongThuoc)
                .HasForeignKey(l => l.MaThuoc)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình decimal precision
            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(hs => hs.ChieuCao)
                .HasPrecision(5, 2);

            modelBuilder.Entity<HoSoSucKhoe>()
                .Property(hs => hs.CanNang)
                .HasPrecision(5, 2);

            modelBuilder.Entity<LichSuBMI>()
                .Property(ls => ls.BMI)
                .HasPrecision(5, 2);

            modelBuilder.Entity<LichSuBMI>()
                .Property(ls => ls.ChieuCao)
                .HasPrecision(5, 2);

            modelBuilder.Entity<LichSuBMI>()
                .Property(ls => ls.CanNang)
                .HasPrecision(5, 2);

            modelBuilder.Entity<GiacNgu>()
                .Property(gn => gn.TongGio)
                .HasPrecision(4, 2);
        }
    }
}
