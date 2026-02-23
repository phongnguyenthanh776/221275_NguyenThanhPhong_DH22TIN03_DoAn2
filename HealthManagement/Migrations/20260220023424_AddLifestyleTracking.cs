using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddLifestyleTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiacNgu",
                columns: table => new
                {
                    MaGiacNgu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    GioNgu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongGio = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    ChatLuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiacNgu", x => x.MaGiacNgu);
                    table.ForeignKey(
                        name: "FK_GiacNgu_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichTap",
                columns: table => new
                {
                    MaLichTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LoaiBaiTap = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CuongDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichTap", x => x.MaLichTap);
                    table.ForeignKey(
                        name: "FK_LichTap_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UongNuoc",
                columns: table => new
                {
                    MaUongNuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    SoMl = table.Column<int>(type: "int", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UongNuoc", x => x.MaUongNuoc);
                    table.ForeignKey(
                        name: "FK_UongNuoc_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuoiTap",
                columns: table => new
                {
                    MaBuoiTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaLichTap = table.Column<int>(type: "int", nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaiTap = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ThoiLuongPhut = table.Column<int>(type: "int", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CamNhan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiTap", x => x.MaBuoiTap);
                    table.ForeignKey(
                        name: "FK_BuoiTap_LichTap_MaLichTap",
                        column: x => x.MaLichTap,
                        principalTable: "LichTap",
                        principalColumn: "MaLichTap",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BuoiTap_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuoiTap_MaLichTap",
                table: "BuoiTap",
                column: "MaLichTap");

            migrationBuilder.CreateIndex(
                name: "IX_BuoiTap_MaNguoiDung",
                table: "BuoiTap",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_GiacNgu_MaNguoiDung",
                table: "GiacNgu",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_LichTap_MaNguoiDung",
                table: "LichTap",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_UongNuoc_MaNguoiDung",
                table: "UongNuoc",
                column: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuoiTap");

            migrationBuilder.DropTable(
                name: "GiacNgu");

            migrationBuilder.DropTable(
                name: "UongNuoc");

            migrationBuilder.DropTable(
                name: "LichTap");
        }
    }
}
