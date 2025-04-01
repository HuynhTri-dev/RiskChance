using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class UpdateThongBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao");

            migrationBuilder.RenameColumn(
                name: "IDNguoiDung",
                table: "ThongBao",
                newName: "IDNguoiNhan");

            migrationBuilder.RenameIndex(
                name: "IX_ThongBao_IDNguoiDung",
                table: "ThongBao",
                newName: "IX_ThongBao_IDNguoiNhan");

            migrationBuilder.AddColumn<string>(
                name: "IDNguoiGui",
                table: "ThongBao",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_IDNguoiGui",
                table: "ThongBao",
                column: "IDNguoiGui");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiGui",
                table: "ThongBao",
                column: "IDNguoiGui",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiNhan",
                table: "ThongBao",
                column: "IDNguoiNhan",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiGui",
                table: "ThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiNhan",
                table: "ThongBao");

            migrationBuilder.DropIndex(
                name: "IX_ThongBao_IDNguoiGui",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "IDNguoiGui",
                table: "ThongBao");

            migrationBuilder.RenameColumn(
                name: "IDNguoiNhan",
                table: "ThongBao",
                newName: "IDNguoiDung");

            migrationBuilder.RenameIndex(
                name: "IX_ThongBao_IDNguoiNhan",
                table: "ThongBao",
                newName: "IX_ThongBao_IDNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
