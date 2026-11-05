using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhiskTakers.Infrastructure;

#nullable disable

namespace WhiskTakers.Infrastructure.Migrations
{
    [DbContext(typeof(WhiskTakersDbContext))]
    [Migration("20261105000200_AddFloorPlanSettings")]
    public partial class AddFloorPlanSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FloorPlanSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AreasJson = table.Column<string>(type: "longtext", nullable: false),
                    TablesJson = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_FloorPlanSettings", x => x.Id))
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "FloorPlanSettings");
    }
}
