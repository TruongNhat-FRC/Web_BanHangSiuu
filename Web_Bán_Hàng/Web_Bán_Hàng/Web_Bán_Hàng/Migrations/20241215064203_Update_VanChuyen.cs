using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Bán_Hàng.Migrations
{
    /// <inheritdoc />
    public partial class Update_VanChuyen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VanChuyens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Phuong_xa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thanhpho = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VanChuyens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VanChuyens");
        }
    }
}
