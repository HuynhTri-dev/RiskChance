using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class MinhChungThanhToan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinhChungThanhToan",
                table: "HopDongDauTu",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinhChungThanhToan",
                table: "HopDongDauTu");
        }
    }
}
