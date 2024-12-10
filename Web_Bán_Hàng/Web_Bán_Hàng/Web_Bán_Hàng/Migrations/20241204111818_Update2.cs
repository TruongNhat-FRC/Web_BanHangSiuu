using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Bán_Hàng.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "DonHangModelID",
                table: "ChiTietDonHangs");

            migrationBuilder.AddColumn<int>(
                name: "DonHangId",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.AddColumn<int>(
                name: "DonHangModelID",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangModelID",
                table: "ChiTietDonHangs",
                column: "DonHangModelID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangModelID",
                table: "ChiTietDonHangs",
                column: "DonHangModelID",
                principalTable: "DonHangs",
                principalColumn: "ID");
        }
    }
}
