using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LieuDung = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLanNgay = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc", x => x.MaThuoc);
                    table.ForeignKey(
                        name: "FK_Thuoc_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichUongThuoc",
                columns: table => new
                {
                    MaLichUong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    GioNhac = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaUong = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichUongThuoc", x => x.MaLichUong);
                    table.ForeignKey(
                        name: "FK_LichUongThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichUongThuoc_MaThuoc",
                table: "LichUongThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_MaNguoiDung",
                table: "Thuoc",
                column: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichUongThuoc");

            migrationBuilder.DropTable(
                name: "Thuoc");
        }
    }
}
