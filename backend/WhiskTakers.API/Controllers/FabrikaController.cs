using Microsoft.AspNetCore.Mvc;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WhiskTakers.Application.DTOs.Fabrika;
using WhiskTakers.Application.DTOs.Puntori;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FabrikaController : ControllerBase
    {
        private readonly WhiskTakersDbContext _context;

        public FabrikaController(WhiskTakersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFabrika()
        {
            var fabrika = await _context.Fabrika
                .Include(f => f.Puntori)
                .ToListAsync();

            var response = fabrika.Select(f => new FabrikaResponseDto
            {
                Id = f.Id,
                EmriFabrikes = f.EmriFabrikes,
                AdresaFabrikes = f.AdresaFabrikes,
                Puntori = f.Puntori.Select(p => new PuntoriResponseDto
                {
                    Id = p.Id,
                    Emri = p.Emri,
                    Mbiemri = p.Mbiemri,
                    Pozita = p.Pozita,
                    FabrikaId = p.FabrikaId,
                    FabrikaEmri = f.EmriFabrikes
                }).ToList()
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFabrika(int id)
        {
            var fabrika = await _context.Fabrika
                .Include(f => f.Puntori)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fabrika == null) return NotFound();

            var response = new FabrikaResponseDto
            {
                Id = fabrika.Id,
                EmriFabrikes = fabrika.EmriFabrikes,
                AdresaFabrikes = fabrika.AdresaFabrikes,
                Puntori = fabrika.Puntori.Select(p => new PuntoriResponseDto
                {
                    Id = p.Id,
                    Emri = p.Emri,
                    Mbiemri = p.Mbiemri,
                    Pozita = p.Pozita,
                    FabrikaId = p.FabrikaId,
                    FabrikaEmri = fabrika.EmriFabrikes
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFabrika(FabrikaCreateDto dto)
        {
            var fabrika = new Fabrika
            {
                EmriFabrikes = dto.EmriFabrikes,
                AdresaFabrikes = dto.AdresaFabrikes
            };

            _context.Fabrika.Add(fabrika);
            await _context.SaveChangesAsync();

            var response = new FabrikaResponseDto
            {
                Id = fabrika.Id,
                EmriFabrikes = fabrika.EmriFabrikes,
                AdresaFabrikes = fabrika.AdresaFabrikes,
                Puntori = new List<PuntoriResponseDto>()
            };

            return CreatedAtAction(nameof(GetFabrika), new { id = fabrika.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFabrika(int id, FabrikaUpdateDto dto)
        {
            var fabrika = await _context.Fabrika.FindAsync(id);
            if (fabrika == null) return NotFound();

            fabrika.EmriFabrikes = dto.EmriFabrikes;
            fabrika.AdresaFabrikes = dto.AdresaFabrikes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFabrika(int id)
        {
            var fabrika = await _context.Fabrika
                .Include(f => f.Puntori)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fabrika == null) return NotFound();

            // Delete associated puntori first
            _context.Puntori.RemoveRange(fabrika.Puntori);
            _context.Fabrika.Remove(fabrika);
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedFabrika()
        {
            // Check if fabrika already exist
            if (await _context.Fabrika.AnyAsync())
            {
                return Ok(new { message = "Fabrika already exist" });
            }

            // Create sample fabrika
            var fabrika = new[]
            {
                new Fabrika { EmriFabrikes = "Fabrika e 1", AdresaFabrikes = "Rruga e mujja, Prishtin" },
                new Fabrika { EmriFabrikes = "Fabrika e 1", AdresaFabrikes = "Rrugatest, Pej" },
                new Fabrika { EmriFabrikes = "Fabrika e 21", AdresaFabrikes = "Rruga , Gjakov" }
            };

            _context.Fabrika.AddRange(fabrika);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sample fabrika created successfully" });
        }
    }
}