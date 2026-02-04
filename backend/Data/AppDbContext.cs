using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NguoiDung> NguoiDung { get; set; }
    public DbSet<ThongTinCaNhan> ThongTinCaNhan { get; set; }
    public DbSet<ChiSoSucKhoe> ChiSoSucKhoe { get; set; }
    public DbSet<LichSuBMI> LichSuBMI { get; set; }
    public DbSet<ThoiQuenSinhHoat> ThoiQuenSinhHoat { get; set; }
    public DbSet<MonAn> MonAn { get; set; }
    public DbSet<ThucDon> ThucDon { get; set; }
    public DbSet<NguyCoBenhLy> NguyCoBenhLy { get; set; }
    public DbSet<CanhBaoSucKhoe> CanhBaoSucKhoe { get; set; }
    public DbSet<GoiYSucKhoeAI> GoiYSucKhoeAI { get; set; }
    public DbSet<LichSuChat> LichSuChat { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình bảng NguoiDung
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.ToTable("NguoiDung", "dbo");
            entity.HasKey(e => e.MaNguoiDung);
            entity.Property(e => e.TenDangNhap).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MatKhau).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        // Cấu hình các bảng khác
        modelBuilder.Entity<ThongTinCaNhan>().ToTable("ThongTinCaNhan", "dbo");
        modelBuilder.Entity<ChiSoSucKhoe>().ToTable("ChiSoSucKhoe", "dbo");
        modelBuilder.Entity<LichSuBMI>().ToTable("LichSuBMI", "dbo");
        modelBuilder.Entity<ThoiQuenSinhHoat>().ToTable("ThoiQuenSinhHoat", "dbo");
        modelBuilder.Entity<MonAn>().ToTable("MonAn", "dbo");
        modelBuilder.Entity<ThucDon>().ToTable("ThucDon", "dbo");
        modelBuilder.Entity<NguyCoBenhLy>().ToTable("NguyCoBenhLy", "dbo");
        modelBuilder.Entity<CanhBaoSucKhoe>().ToTable("CanhBaoSucKhoe", "dbo");
        modelBuilder.Entity<GoiYSucKhoeAI>().ToTable("GoiYSucKhoeAI", "dbo");
        modelBuilder.Entity<LichSuChat>().ToTable("LichSuChat", "dbo");

        // Configure LichSuChat
        modelBuilder.Entity<LichSuChat>()
            .HasKey(c => c.MaChat);

        modelBuilder.Entity<LichSuChat>()
            .Property(c => c.MaChat)
            .ValueGeneratedOnAdd();
    }
}
