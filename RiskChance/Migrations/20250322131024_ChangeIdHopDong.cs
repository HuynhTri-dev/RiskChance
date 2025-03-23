using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoiDungHopDong",
                table: "HopDongDauTu",
                newName: "NoiDung");

            migrationBuilder.RenameColumn(
                name: "AnhXacNhan",
                table: "HopDongDauTu",
                newName: "FileUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoiDung",
                table: "HopDongDauTu",
                newName: "NoiDungHopDong");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "HopDongDauTu",
                newName: "AnhXacNhan");
        }
    }
}
