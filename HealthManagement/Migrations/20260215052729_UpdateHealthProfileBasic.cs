using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHealthProfileBasic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiUng",
                table: "HoSoSucKhoe");

            migrationBuilder.DropColumn(
                name: "ThuocDangDung",
                table: "HoSoSucKhoe");

            migrationBuilder.DropColumn(
                name: "TienSuBenhLy",
                table: "HoSoSucKhoe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiUng",
                table: "HoSoSucKhoe",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThuocDangDung",
                table: "HoSoSucKhoe",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TienSuBenhLy",
                table: "HoSoSucKhoe",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
