using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WhiskTakers.Infrastructure;

#nullable disable

namespace WhiskTakers.Infrastructure.Migrations
{
    [DbContext(typeof(WhiskTakersDbContext))]
    [Migration("20261105000300_AddOrderPreparationTimer")]
    public partial class AddOrderPreparationTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(name: "PreparationStartedAtUtc", table: "Orders", type: "datetime(6)", nullable: true);
            migrationBuilder.AddColumn<int>(name: "TimerExtensionMinutes", table: "Orders", type: "int", nullable: false, defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PreparationStartedAtUtc", table: "Orders");
            migrationBuilder.DropColumn(name: "TimerExtensionMinutes", table: "Orders");
        }
    }
}
