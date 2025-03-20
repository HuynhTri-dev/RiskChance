using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReferencesBinhLuanAndTinTuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_IDNguoiDung",
                table: "DanhGiaStartup");

            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_NguoiDungId",
                table: "DanhGiaStartup",
                column: "NguoiDungId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_IDNguoiDung",
                table: "DanhGiaStartup",
                column: "IDNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
