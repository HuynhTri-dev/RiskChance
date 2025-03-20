using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class AddBinhLuanAndChangeReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_Startup_IDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_Startup_StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_GiayTo_Startup_IDStartup",
                table: "GiayTo");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongDauTu_AspNetUsers_IDNguoiDung",
                table: "HopDongDauTu");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongDauTu_Startup_IDStartup",
                table: "HopDongDauTu");

            migrationBuilder.DropForeignKey(
                name: "FK_Startup_AspNetUsers_IDNguoiDung",
                table: "Startup");

            migrationBuilder.DropForeignKey(
                name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                table: "ThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                table: "TinNhan");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTuc_AspNetUsers_IDNguoiDung",
                table: "TinTuc");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropIndex(
                name: "IX_DanhGiaStartup_StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "HopDongDauTu");

            migrationBuilder.DropColumn(
                name: "NguoiDungId",
                table: "DanhGiaStartup");

            migrationBuilder.DropColumn(
                name: "StartupIDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "DanhGiaStartup",
                newName: "IDDanhGia");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayKyKet",
                table: "HopDongDauTu",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IDStartup",
                table: "HopDongDauTu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiKyKet",
                table: "HopDongDauTu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "BinhLuanTinTuc",
                columns: table => new
                {
                    IDBinhLuan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiemDanhGia = table.Column<float>(type: "real", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBinhLuan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDTinTuc = table.Column<int>(type: "int", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NguoiDungId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinhLuanTinTuc", x => x.IDBinhLuan);
                    table.ForeignKey(
                        name: "FK_BinhLuanTinTuc_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BinhLuanTinTuc_AspNetUsers_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BinhLuanTinTuc_TinTuc_IDTinTuc",
                        column: x => x.IDTinTuc,
                        principalTable: "TinTuc",
                        principalColumn: "IDTinTuc",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_BinhLuanTinTuc_IDNguoiDung",
                table: "BinhLuanTinTuc",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanTinTuc_IDTinTuc",
                table: "BinhLuanTinTuc",
                column: "IDTinTuc");

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuanTinTuc_NguoiDungId",
                table: "BinhLuanTinTuc",
                column: "NguoiDungId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_Startup_IDStartup",
                table: "DanhGiaStartup",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GiayTo_Startup_IDStartup",
                table: "GiayTo",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongDauTu_AspNetUsers_IDNguoiDung",
                table: "HopDongDauTu",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongDauTu_Startup_IDStartup",
                table: "HopDongDauTu",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Startup_AspNetUsers_IDNguoiDung",
                table: "Startup",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_TinTuc_AspNetUsers_IDNguoiDung",
                table: "TinTuc",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_DanhGiaStartup_Startup_IDStartup",
                table: "DanhGiaStartup");

            migrationBuilder.DropForeignKey(
                name: "FK_GiayTo_Startup_IDStartup",
                table: "GiayTo");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongDauTu_AspNetUsers_IDNguoiDung",
                table: "HopDongDauTu");

            migrationBuilder.DropForeignKey(
                name: "FK_HopDongDauTu_Startup_IDStartup",
                table: "HopDongDauTu");

            migrationBuilder.DropForeignKey(
                name: "FK_Startup_AspNetUsers_IDNguoiDung",
                table: "Startup");

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
                name: "FK_TinTuc_AspNetUsers_IDNguoiDung",
                table: "TinTuc");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTuc_AspNetUsers_NguoiDungId",
                table: "TinTuc");

            migrationBuilder.DropForeignKey(
                name: "FK_TinTucHashtag_TinTucHashtag_TinTucHashtagIDTinTuc_TinTucHashtagIDHashtag",
                table: "TinTucHashtag");

            migrationBuilder.DropTable(
                name: "BinhLuanTinTuc");

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
                name: "TrangThaiKyKet",
                table: "HopDongDauTu");

            migrationBuilder.RenameColumn(
                name: "IDDanhGia",
                table: "DanhGiaStartup",
                newName: "ID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayKyKet",
                table: "HopDongDauTu",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "IDStartup",
                table: "HopDongDauTu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "HopDongDauTu",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IDNguoiDung",
                table: "DanhGiaStartup",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
                name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                table: "DanhGiaStartup",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_AspNetUsers_NguoiDungId",
                table: "DanhGiaStartup",
                column: "NguoiDungId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_Startup_IDStartup",
                table: "DanhGiaStartup",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhGiaStartup_Startup_StartupIDStartup",
                table: "DanhGiaStartup",
                column: "StartupIDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup");

            migrationBuilder.AddForeignKey(
                name: "FK_GiayTo_Startup_IDStartup",
                table: "GiayTo",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongDauTu_AspNetUsers_IDNguoiDung",
                table: "HopDongDauTu",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HopDongDauTu_Startup_IDStartup",
                table: "HopDongDauTu",
                column: "IDStartup",
                principalTable: "Startup",
                principalColumn: "IDStartup");

            migrationBuilder.AddForeignKey(
                name: "FK_Startup_AspNetUsers_IDNguoiDung",
                table: "Startup",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_TinTuc_AspNetUsers_IDNguoiDung",
                table: "TinTuc",
                column: "IDNguoiDung",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
