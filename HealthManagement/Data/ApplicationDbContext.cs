using HealthManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthManagement.Data
{
    /// <summary>
    /// DbContext chính của ứng dụng - quản lý 12 bảng
    /// Kế thừa từ IdentityDbContext để sử dụng ASP.NET Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho 12 bảng
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<VaiTro> VaiTro { get; set; }
        public DbSet<HoSoSucKhoe> HoSoSucKhoe { get; set; }
        public DbSet<ChiSoSucKhoe> ChiSoSucKhoe { get; set; }
        public DbSet<LichSuBMI> LichSuBMI { get; set; }
        public DbSet<ThucDon> ThucDon { get; set; }
        public DbSet<KeHoachAnUong> KeHoachAnUong { get; set; }
        public DbSet<ChiTietKeHoachAn> ChiTietKeHoachAn { get; set; }
        public DbSet<DuDoanAI> DuDoanAI { get; set; }
        public DbSet<LichSuChatBot> LichSuChatBot { get; set; }
        public DbSet<BaiVietSucKhoe> BaiVietSucKhoe { get; set; }
        public DbSet<PhanHoi> PhanHoi { get; set; }

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

            // NguoiDung - KeHoachAnUong (1-n)
            modelBuilder.Entity<KeHoachAnUong>()
                .HasOne(kh => kh.NguoiDung)
                .WithMany(nd => nd.KeHoachAnUong)
                .HasForeignKey(kh => kh.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // KeHoachAnUong - ChiTietKeHoachAn (1-n)
            modelBuilder.Entity<ChiTietKeHoachAn>()
                .HasOne(ct => ct.KeHoachAnUong)
                .WithMany(kh => kh.ChiTietKeHoachAn)
                .HasForeignKey(ct => ct.MaKeHoach)
                .OnDelete(DeleteBehavior.Cascade);

            // ThucDon - ChiTietKeHoachAn (1-n)
            modelBuilder.Entity<ChiTietKeHoachAn>()
                .HasOne(ct => ct.ThucDon)
                .WithMany(td => td.ChiTietKeHoachAn)
                .HasForeignKey(ct => ct.MaMonAn)
                .OnDelete(DeleteBehavior.Restrict);

            // NguoiDung - DuDoanAI (1-n)
            modelBuilder.Entity<DuDoanAI>()
                .HasOne(dd => dd.NguoiDung)
                .WithMany(nd => nd.DuDoanAI)
                .HasForeignKey(dd => dd.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - LichSuChatBot (1-n)
            modelBuilder.Entity<LichSuChatBot>()
                .HasOne(ls => ls.NguoiDung)
                .WithMany(nd => nd.LichSuChatBot)
                .HasForeignKey(ls => ls.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - PhanHoi (1-n)
            modelBuilder.Entity<PhanHoi>()
                .HasOne(ph => ph.NguoiDung)
                .WithMany(nd => nd.PhanHoi)
                .HasForeignKey(ph => ph.MaNguoiDung)
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

            modelBuilder.Entity<ThucDon>()
                .Property(td => td.Calo)
                .HasPrecision(7, 2);
        }
    }
}
