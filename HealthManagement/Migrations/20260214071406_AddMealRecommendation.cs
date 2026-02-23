using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddMealRecommendation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoiYThucDon",
                columns: table => new
                {
                    MaGoiY = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaDuDoan = table.Column<int>(type: "int", nullable: false),
                    LoaiBenhDuDoan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CacMonAn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KhuyanCao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    KeHoachAn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_GoiYThucDon_MaDuDoan",
                table: "GoiYThucDon",
                column: "MaDuDoan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoiYThucDon");
        }
    }
}
