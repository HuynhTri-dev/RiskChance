using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiskChance.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hashtag",
                columns: table => new
                {
                    IDHashtag = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHashtag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashtag", x => x.IDHashtag);
                });

            migrationBuilder.CreateTable(
                name: "LinhVuc",
                columns: table => new
                {
                    IDLinhVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLinhVuc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhVuc", x => x.IDLinhVuc);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    IDNoti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.IDNoti);
                    table.ForeignKey(
                        name: "FK_ThongBao_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TinNhan",
                columns: table => new
                {
                    IDTinNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDNguoiGui = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IDNguoiNhan = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhan", x => x.IDTinNhan);
                    table.ForeignKey(
                        name: "FK_TinNhan_AspNetUsers_IDNguoiGui",
                        column: x => x.IDNguoiGui,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TinNhan_AspNetUsers_IDNguoiNhan",
                        column: x => x.IDNguoiNhan,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TinTuc",
                columns: table => new
                {
                    IDTinTuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImgTinTuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTuc", x => x.IDTinTuc);
                    table.ForeignKey(
                        name: "FK_TinTuc_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Startup",
                columns: table => new
                {
                    IDStartup = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenStartup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDLinhVuc = table.Column<int>(type: "int", nullable: true),
                    MucTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhanTramCoPhan = table.Column<double>(type: "float", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiXetDuyet = table.Column<int>(type: "int", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Startup", x => x.IDStartup);
                    table.ForeignKey(
                        name: "FK_Startup_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Startup_LinhVuc_IDLinhVuc",
                        column: x => x.IDLinhVuc,
                        principalTable: "LinhVuc",
                        principalColumn: "IDLinhVuc",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TinTucHashtag",
                columns: table => new
                {
                    IDTinTuc = table.Column<int>(type: "int", nullable: false),
                    IDHashtag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTucHashtag", x => new { x.IDTinTuc, x.IDHashtag });
                    table.ForeignKey(
                        name: "FK_TinTucHashtag_Hashtag_IDHashtag",
                        column: x => x.IDHashtag,
                        principalTable: "Hashtag",
                        principalColumn: "IDHashtag",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TinTucHashtag_TinTuc_IDTinTuc",
                        column: x => x.IDTinTuc,
                        principalTable: "TinTuc",
                        principalColumn: "IDTinTuc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGiaStartup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiemDanhGia = table.Column<int>(type: "int", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDStartup = table.Column<int>(type: "int", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaStartup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DanhGiaStartup_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGiaStartup_Startup_IDStartup",
                        column: x => x.IDStartup,
                        principalTable: "Startup",
                        principalColumn: "IDStartup");
                });

            migrationBuilder.CreateTable(
                name: "GiayTo",
                columns: table => new
                {
                    IDGiayTo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenGiayTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDStartup = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiayTo", x => x.IDGiayTo);
                    table.ForeignKey(
                        name: "FK_GiayTo_Startup_IDStartup",
                        column: x => x.IDStartup,
                        principalTable: "Startup",
                        principalColumn: "IDStartup",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HopDongDauTu",
                columns: table => new
                {
                    IDHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnhXacNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKyKet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhanTramLoiNhuan = table.Column<double>(type: "float", nullable: true),
                    NoiDungHopDong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDStartup = table.Column<int>(type: "int", nullable: false),
                    IDNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDongDauTu", x => x.IDHopDong);
                    table.ForeignKey(
                        name: "FK_HopDongDauTu_AspNetUsers_IDNguoiDung",
                        column: x => x.IDNguoiDung,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HopDongDauTu_Startup_IDStartup",
                        column: x => x.IDStartup,
                        principalTable: "Startup",
                        principalColumn: "IDStartup");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_IDNguoiDung",
                table: "DanhGiaStartup",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaStartup_IDStartup",
                table: "DanhGiaStartup",
                column: "IDStartup");

            migrationBuilder.CreateIndex(
                name: "IX_GiayTo_IDStartup",
                table: "GiayTo",
                column: "IDStartup");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongDauTu_IDNguoiDung",
                table: "HopDongDauTu",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_HopDongDauTu_IDStartup",
                table: "HopDongDauTu",
                column: "IDStartup");

            migrationBuilder.CreateIndex(
                name: "IX_Startup_IDLinhVuc",
                table: "Startup",
                column: "IDLinhVuc");

            migrationBuilder.CreateIndex(
                name: "IX_Startup_IDNguoiDung",
                table: "Startup",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_IDNguoiDung",
                table: "ThongBao",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_IDNguoiGui",
                table: "TinNhan",
                column: "IDNguoiGui");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhan_IDNguoiNhan",
                table: "TinNhan",
                column: "IDNguoiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_TinTuc_IDNguoiDung",
                table: "TinTuc",
                column: "IDNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TinTucHashtag_IDHashtag",
                table: "TinTucHashtag",
                column: "IDHashtag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DanhGiaStartup");

            migrationBuilder.DropTable(
                name: "GiayTo");

            migrationBuilder.DropTable(
                name: "HopDongDauTu");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "TinNhan");

            migrationBuilder.DropTable(
                name: "TinTucHashtag");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Startup");

            migrationBuilder.DropTable(
                name: "Hashtag");

            migrationBuilder.DropTable(
                name: "TinTuc");

            migrationBuilder.DropTable(
                name: "LinhVuc");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
