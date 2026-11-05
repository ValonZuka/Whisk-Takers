using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhiskTakers.Infrastructure;

#nullable disable

namespace WhiskTakers.Infrastructure.Migrations
{
    [DbContext(typeof(WhiskTakersDbContext))]
    [Migration("20261105000500_AddProductAvailability")]
    public partial class AddProductAvailability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql("UPDATE Products SET IsAvailable = 0 WHERE LOWER(TRIM(Name)) IN ('valon', 'testi valon')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsAvailable", table: "Products");
        }
    }
}
