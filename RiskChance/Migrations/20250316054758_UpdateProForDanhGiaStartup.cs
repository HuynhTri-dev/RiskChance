using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProForDanhGiaStartup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "DiemDanhGia",
                table: "DanhGiaStartup",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartupIDStartup",
                table: "DanhGiaStartup",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_NguoiDungId",
                table: "DanhGiaStartup",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_StartupIDStartup",
                table: "DanhGiaStartup",
                column: "StartupIDStartup");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_Startup_StartupIDStartup",
                table: "DanhGiaStartup",
                column: "StartupIDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_Startup_StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropColumn(
                name: "StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "DiemDanhGia",
                table: "DanhGiaStartup",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
