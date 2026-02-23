using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class SupportMultipleDiseases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiDuDoan",
                table: "DuDoanAI");

            migrationBuilder.AddColumn<int>(
                name: "LoaiBenhDuDoan",
                table: "DuDoanAI",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiBenhDuDoan",
                table: "DuDoanAI");

            migrationBuilder.AddColumn<string>(
                name: "LoaiDuDoan",
                table: "DuDoanAI",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
