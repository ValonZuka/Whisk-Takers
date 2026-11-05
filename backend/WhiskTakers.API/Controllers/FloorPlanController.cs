using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskTakers.Core.Entities;
using WhiskTakers.Infrastructure;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorPlanController : ControllerBase
    {
        private readonly WhiskTakersDbContext _context;

        public FloorPlanController(WhiskTakersDbContext context) => _context = context;

        [HttpGet]
        [Authorize(Roles = "admin,waiter,chef")]
        public async Task<IActionResult> Get()
        {
            var plan = await _context.FloorPlanSettings.FindAsync(1);
            if (plan == null) return NotFound();
            return Ok(new
            {
                areas = JsonSerializer.Deserialize<List<FloorAreaDto>>(plan.AreasJson) ?? new(),
                tables = JsonSerializer.Deserialize<List<FloorTableDto>>(plan.TablesJson) ?? new()
            });
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Save([FromBody] FloorPlanUpdateDto dto)
        {
            if (dto.Areas == null || dto.Areas.Count == 0 || dto.Tables == null ||
                dto.Areas.Any(area => string.IsNullOrWhiteSpace(area.Name) || string.IsNullOrWhiteSpace(area.Id)) ||
                dto.Tables.Any(table => string.IsNullOrWhiteSpace(table.Name) || table.Seats < 1 || table.Width < 70 || table.Height < 60 || !dto.Areas.Any(area => area.Id == table.AreaId)))
                return BadRequest("The floor plan must include named areas and tables with valid sizes and seats.");

            var plan = await _context.FloorPlanSettings.FindAsync(1);
            if (plan == null)
            {
                plan = new FloorPlanSettings { Id = 1 };
                _context.FloorPlanSettings.Add(plan);
            }
            plan.AreasJson = JsonSerializer.Serialize(dto.Areas);
            plan.TablesJson = JsonSerializer.Serialize(dto.Tables);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public sealed class FloorPlanUpdateDto
    {
        public List<FloorAreaDto> Areas { get; set; } = new();
        public List<FloorTableDto> Tables { get; set; } = new();
    }

    public sealed class FloorAreaDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#0f766e";
    }

    public sealed class FloorTableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AreaId { get; set; } = string.Empty;
        public int Seats { get; set; } = 4;
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; } = 132;
        public int Height { get; set; } = 96;
    }
}
