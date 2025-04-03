using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeForgeinKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToanLoiNhuans_HopDongDauTu_HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToanLoiNhuans_HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans");

            migrationBuilder.DropColumn(
                name: "HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanLoiNhuans_HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans",
                column: "HopDongDauTuIDHopDong");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToanLoiNhuans_HopDongDauTu_HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans",
                column: "HopDongDauTuIDHopDong",
                principalTable: "HopDongDauTu",
                principalColumn: "IDHopDong");
        }
    }
}
