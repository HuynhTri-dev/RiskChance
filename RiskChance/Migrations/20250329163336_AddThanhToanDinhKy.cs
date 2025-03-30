using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class AddThanhToanDinhKy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ThanhToan",
                table: "HopDongDauTu",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IDStartup",
                table: "GiayTo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ThanhToanLoiNhuans",
                columns: table => new
                {
                    IdThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDHopDong = table.Column<int>(type: "int", nullable: true),
                    HopDongDauTuIDHopDong = table.Column<int>(type: "int", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToanLoiNhuans", x => x.IdThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToanLoiNhuans_HopDongDauTu_HopDongDauTuIDHopDong",
                        column: x => x.HopDongDauTuIDHopDong,
                        principalTable: "HopDongDauTu",
                        principalColumn: "IDHopDong");
                    table.ForeignKey(
                        name: "FK_ThanhToanLoiNhuans_HopDongDauTu_IDHopDong",
                        column: x => x.IDHopDong,
                        principalTable: "HopDongDauTu",
                        principalColumn: "IDHopDong",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanLoiNhuans_HopDongDauTuIDHopDong",
                table: "ThanhToanLoiNhuans",
                column: "HopDongDauTuIDHopDong");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanLoiNhuans_IDHopDong",
                table: "ThanhToanLoiNhuans",
                column: "IDHopDong");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThanhToanLoiNhuans");

            migrationBuilder.DropColumn(
                name: "ThanhToan",
                table: "HopDongDauTu");

            migrationBuilder.AlterColumn<int>(
                name: "IDStartup",
                table: "GiayTo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
