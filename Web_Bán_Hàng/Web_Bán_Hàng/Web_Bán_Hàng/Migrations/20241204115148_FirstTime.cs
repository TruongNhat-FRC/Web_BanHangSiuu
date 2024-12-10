using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Bán_Hàng.Migrations
{
    /// <inheritdoc />
    public partial class FirstTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_Products_ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.AlterColumn<string>(
                name: "MaDonHang",
                table: "DonHangs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "ChiTietDonHangs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaDonHang",
                table: "ChiTietDonHangs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DonHangs_MaDonHang",
                table: "DonHangs",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaDonHang",
                table: "DonHangs",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_MaDonHang",
                table: "ChiTietDonHangs",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_MaSanPham",
                table: "ChiTietDonHangs",
                column: "MaSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_MaDonHang",
                table: "ChiTietDonHangs",
                column: "MaDonHang",
                principalTable: "DonHangs",
                principalColumn: "MaDonHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_Products_MaSanPham",
                table: "ChiTietDonHangs",
                column: "MaSanPham",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_MaDonHang",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_Products_MaSanPham",
                table: "ChiTietDonHangs");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DonHangs_MaDonHang",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_MaDonHang",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_MaDonHang",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_MaSanPham",
                table: "ChiTietDonHangs");

            migrationBuilder.AlterColumn<string>(
                name: "MaDonHang",
                table: "DonHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "ChiTietDonHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MaDonHang",
                table: "ChiTietDonHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "DonHangId",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ChiTietDonHangs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_ProductId",
                table: "ChiTietDonHangs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_Products_ProductId",
                table: "ChiTietDonHangs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
