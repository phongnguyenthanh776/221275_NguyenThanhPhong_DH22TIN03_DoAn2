using HealthManagement.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260223090000_RemoveThoiGianDocFromTinTucSucKhoe")]
    public partial class RemoveThoiGianDocFromTinTucSucKhoe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianDoc",
                table: "TinTucSucKhoe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThoiGianDoc",
                table: "TinTucSucKhoe",
                type: "int",
                nullable: false,
                defaultValue: 5);
        }
    }
}
