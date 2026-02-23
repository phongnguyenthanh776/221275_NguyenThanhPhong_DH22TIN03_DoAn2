using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthManagement.Migrations
{
    /// <inheritdoc />
    public partial class RenameWaterReminderToNhacUongNuoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterReminder_NguoiDung_MaNguoiDung",
                table: "WaterReminder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaterReminder",
                table: "WaterReminder");

            migrationBuilder.RenameTable(
                name: "WaterReminder",
                newName: "NhacUongNuoc");

            migrationBuilder.RenameIndex(
                name: "IX_WaterReminder_MaNguoiDung",
                table: "NhacUongNuoc",
                newName: "IX_NhacUongNuoc_MaNguoiDung");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhacUongNuoc",
                table: "NhacUongNuoc",
                column: "MaNhac");

            migrationBuilder.AddForeignKey(
                name: "FK_NhacUongNuoc_NguoiDung_MaNguoiDung",
                table: "NhacUongNuoc",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhacUongNuoc_NguoiDung_MaNguoiDung",
                table: "NhacUongNuoc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhacUongNuoc",
                table: "NhacUongNuoc");

            migrationBuilder.RenameTable(
                name: "NhacUongNuoc",
                newName: "WaterReminder");

            migrationBuilder.RenameIndex(
                name: "IX_NhacUongNuoc_MaNguoiDung",
                table: "WaterReminder",
                newName: "IX_WaterReminder_MaNguoiDung");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaterReminder",
                table: "WaterReminder",
                column: "MaNhac");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterReminder_NguoiDung_MaNguoiDung",
                table: "WaterReminder",
                column: "MaNguoiDung",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
