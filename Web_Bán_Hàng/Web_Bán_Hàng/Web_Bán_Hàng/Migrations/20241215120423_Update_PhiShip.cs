using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Bán_Hàng.Migrations
{
    /// <inheritdoc />
    public partial class Update_PhiShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PhiShip",
                table: "DonHangs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhiShip",
                table: "DonHangs");
        }
    }
}
