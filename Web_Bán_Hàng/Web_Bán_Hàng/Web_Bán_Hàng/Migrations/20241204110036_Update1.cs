using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Bán_Hàng.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "ChiTietDonHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DonHangModelID",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ChiTietDonHangs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangModelID",
                table: "ChiTietDonHangs",
                column: "DonHangModelID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_ProductId",
                table: "ChiTietDonHangs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangModelID",
                table: "ChiTietDonHangs",
                column: "DonHangModelID",
                principalTable: "DonHangs",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_Products_ProductId",
                table: "ChiTietDonHangs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_Products_ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ChiTietDonHangs");

            migrationBuilder.AlterColumn<int>(
                name: "MaSanPham",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
