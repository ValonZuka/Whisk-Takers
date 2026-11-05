using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhiskTakers.Infrastructure;

#nullable disable

namespace WhiskTakers.Infrastructure.Migrations
{
    [DbContext(typeof(WhiskTakersDbContext))]
    [Migration("20261105000400_AddProductImageUrl")]
    public partial class AddProductImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "ImageUrl", table: "Products", type: "longtext", nullable: false, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ImageUrl", table: "Products");
        }
    }
}
