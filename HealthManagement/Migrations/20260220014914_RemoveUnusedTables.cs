using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiVietSucKhoe");

            migrationBuilder.DropTable(
                name: "ChiTietKeHoachAn");

            migrationBuilder.DropTable(
                name: "GoiYThucDon");

            migrationBuilder.DropTable(
                name: "LichSuChatBot");

            migrationBuilder.DropTable(
                name: "PhanHoi");

            migrationBuilder.DropTable(
                name: "KeHoachAnUong");

            migrationBuilder.DropTable(
                name: "ThucDon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaiVietSucKhoe",
                columns: table => new
                {
                    MaBaiViet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LuotXem = table.Column<int>(type: "int", nullable: false),
                    MoTaNgan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TacGia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TieuDe = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiVietSucKhoe", x => x.MaBaiViet);
                });

            migrationBuilder.CreateTable(
                name: "GoiYThucDon",
                columns: table => new
                {
                    MaGoiY = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuDoan = table.Column<int>(type: "int", nullable: false),
                    CacMonAn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KeHoachAn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KhuyanCao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LoaiBenhDuDoan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NgayGoiY = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoiYThucDon", x => x.MaGoiY);
                    table.ForeignKey(
                        name: "FK_GoiYThucDon_DuDoanAI_MaDuDoan",
                        column: x => x.MaDuDoan,
                        principalTable: "DuDoanAI",
                        principalColumn: "MaDuDoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeHoachAnUong",
                columns: table => new
                {
                    MaKeHoach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MucTieuCalo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenKeHoach = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeHoachAnUong", x => x.MaKeHoach);
                    table.ForeignKey(
                        name: "FK_KeHoachAnUong_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuChatBot",
                columns: table => new
                {
                    MaLichSuChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    CauHoi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CauTraLoi = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    HuuIch = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuChatBot", x => x.MaLichSuChat);
                    table.ForeignKey(
                        name: "FK_LichSuChatBot_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanHoi",
                columns: table => new
                {
                    MaPhanHoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    DaXuLy = table.Column<bool>(type: "bit", nullable: false),
                    DanhGia = table.Column<int>(type: "int", nullable: true),
                    LoaiPhanHoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraLoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TraLoi = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanHoi", x => x.MaPhanHoi);
                    table.ForeignKey(
                        name: "FK_PhanHoi_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThucDon",
                columns: table => new
                {
                    MaMonAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calo = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    Carbs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiMonAn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Protein = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenMonAn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThucDon", x => x.MaMonAn);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKeHoachAn",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKeHoach = table.Column<int>(type: "int", nullable: false),
                    MaMonAn = table.Column<int>(type: "int", nullable: false),
                    BuaAn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KhoiLuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayAn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKeHoachAn", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietKeHoachAn_KeHoachAnUong_MaKeHoach",
                        column: x => x.MaKeHoach,
                        principalTable: "KeHoachAnUong",
                        principalColumn: "MaKeHoach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietKeHoachAn_ThucDon_MaMonAn",
                        column: x => x.MaMonAn,
                        principalTable: "ThucDon",
                        principalColumn: "MaMonAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKeHoachAn_MaKeHoach",
                table: "ChiTietKeHoachAn",
                column: "MaKeHoach");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKeHoachAn_MaMonAn",
                table: "ChiTietKeHoachAn",
                column: "MaMonAn");

            migrationBuilder.CreateIndex(
                name: "IX_GoiYThucDon_MaDuDoan",
                table: "GoiYThucDon",
                column: "MaDuDoan");

            migrationBuilder.CreateIndex(
                name: "IX_KeHoachAnUong_MaNguoiDung",
                table: "KeHoachAnUong",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChatBot_MaNguoiDung",
                table: "LichSuChatBot",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhanHoi_MaNguoiDung",
                table: "PhanHoi",
                column: "MaNguoiDung");
        }
    }
}
