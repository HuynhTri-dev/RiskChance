using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuanTinTuc_AspNetUsers_NguoiDungId",
                table: "BinhLuanTinTuc");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_NguoiDungId",
                table: "ThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                table: "TinNhan");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhan_AspNetUsers_NguoiGuiId",
                table: "TinNhan");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTuc_AspNetUsers_NguoiDungId",
                table: "TinTuc");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTucHashtag_TinTucHashtag_TinTucHashtagIDTinTuc_TinTucHashtagIDHashtag",
                table: "TinTucHashtag");

            migrationBuilder.DropIndex(
                name: "IX_TinTucHashtag_TinTucHashtagIDTinTuc_TinTucHashtagIDHashtag",
                table: "TinTucHashtag");

            migrationBuilder.DropIndex(
                name: "IX_TinTuc_NguoiDungId",
                table: "TinTuc");

            migrationBuilder.DropIndex(
                name: "IX_TinNhan_NguoiGuiId",
                table: "TinNhan");

            migrationBuilder.DropIndex(
                name: "IX_ThongBao_NguoiDungId",
                table: "ThongBao");

            migrationBuilder.DropIndex(
                name: "IX_BinhLuanTinTuc_NguoiDungId",
                table: "BinhLuanTinTuc");

            migrationBuilder.DropColumn(
                name: "TinTucHashtagIDHashtag",
                table: "TinTucHashtag");

            migrationBuilder.DropColumn(
                name: "TinTucHashtagIDTinTuc",
                table: "TinTucHashtag");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "TinTuc");

            migrationBuilder.DropColumn(
                name: "NguoiGuiId",
                table: "TinNhan");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "BinhLuanTinTuc");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                table: "TinNhan",
                column: "IDNguoiGui",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                table: "TinNhan");

            migrationBuilder.AddColumn<int>(
                name: "TinTucHashtagIDHashtag",
                table: "TinTucHashtag",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TinTucHashtagIDTinTuc",
                table: "TinTucHashtag",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "TinTuc",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiGuiId",
                table: "TinNhan",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "ThongBao",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDungId",
                table: "BinhLuanTinTuc",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TinTucHashtag_TinTucHashtagIDTinTuc_TinTucHashtagIDHashtag",
                table: "TinTucHashtag",
                columns: new[] { "TinTucHashtagIDTinTuc", "TinTucHashtagIDHashtag" });

            migrationBuilder.CreateIndex(
                name: "IX_TinTuc_NguoiDungId",
                table: "TinTuc",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_NguoiGuiId",
                table: "TinNhan",
                column: "NguoiGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_NguoiDungId",
                table: "ThongBao",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanTinTuc_NguoiDungId",
                table: "BinhLuanTinTuc",
                column: "NguoiDungId");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuanTinTuc_AspNetUsers_NguoiDungId",
                table: "BinhLuanTinTuc",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ThongBao_AspNetUsers_NguoiDungId",
                table: "ThongBao",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                table: "TinNhan",
                column: "IDNguoiGui",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhan_AspNetUsers_NguoiGuiId",
                table: "TinNhan",
                column: "NguoiGuiId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TinTuc_AspNetUsers_NguoiDungId",
                table: "TinTuc",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TinTucHashtag_TinTucHashtag_TinTucHashtagIDTinTuc_TinTucHashtagIDHashtag",
                table: "TinTucHashtag",
                columns: new[] { "TinTucHashtagIDTinTuc", "TinTucHashtagIDHashtag" },
                principalTable: "TinTucHashtag",
                principalColumns: new[] { "IDTinTuc", "IDHashtag" });
        }
    }
}
