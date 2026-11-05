using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhiskTakers.Infrastructure;

#nullable disable

namespace WhiskTakers.Infrastructure.Migrations
{
    [DbContext(typeof(WhiskTakersDbContext))]
    [Migration("20261105000100_AddProductCostsAndPrepTimes")]
    public partial class AddProductCostsAndPrepTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(name: "UnitCost", table: "Products", type: "double", nullable: false, defaultValue: 0d);
            migrationBuilder.AddColumn<int>(name: "PrepMinutes", table: "Products", type: "int", nullable: false, defaultValue: 10);
            migrationBuilder.AddColumn<double>(name: "UnitCostAtOrder", table: "OrderItems", type: "double", nullable: false, defaultValue: 0d);
            migrationBuilder.AddColumn<double>(name: "UnitPriceAtOrder", table: "OrderItems", type: "double", nullable: false, defaultValue: 0d);
            migrationBuilder.AddColumn<int>(name: "PrepMinutesAtOrder", table: "OrderItems", type: "int", nullable: false, defaultValue: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "UnitCost", table: "Products");
            migrationBuilder.DropColumn(name: "PrepMinutes", table: "Products");
            migrationBuilder.DropColumn(name: "UnitCostAtOrder", table: "OrderItems");
            migrationBuilder.DropColumn(name: "UnitPriceAtOrder", table: "OrderItems");
            migrationBuilder.DropColumn(name: "PrepMinutesAtOrder", table: "OrderItems");
        }
    }
}
