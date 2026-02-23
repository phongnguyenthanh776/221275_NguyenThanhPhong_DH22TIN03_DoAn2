using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFitnessTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuoiTap");

            migrationBuilder.DropTable(
                name: "LichTap");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichTap",
                columns: table => new
                {
                    MaLichTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    CuongDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiBaiTap = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TieuDe = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
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
                name: "BuoiTap",
                columns: table => new
                {
                    MaBuoiTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLichTap = table.Column<int>(type: "int", nullable: true),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    BaiTap = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    CamNhan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiLuongPhut = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
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
                name: "IX_LichTap_MaNguoiDung",
                table: "LichTap",
                column: "MaNguoiDung");
        }
    }
}
