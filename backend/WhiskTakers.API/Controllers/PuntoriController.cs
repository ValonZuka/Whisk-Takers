using Microsoft.AspNetCore.Mvc;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WhiskTakers.Application.DTOs.Puntori;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuntoriController : ControllerBase
    {
        private readonly WhiskTakersDbContext _context;

        public PuntoriController(WhiskTakersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPuntori()
        {
            var puntori = await _context.Puntori
                .Include(p => p.Fabrika)
                .ToListAsync();

            var response = puntori.Select(p => new PuntoriResponseDto
            {
                Id = p.Id,
                Emri = p.Emri,
                Mbiemri = p.Mbiemri,
                Pozita = p.Pozita,
                FabrikaId = p.FabrikaId,
                FabrikaEmri = p.Fabrika?.EmriFabrikes
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPuntori(int id)
        {
            var puntori = await _context.Puntori
                .Include(p => p.Fabrika)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (puntori == null) return NotFound();

            var response = new PuntoriResponseDto
            {
                Id = puntori.Id,
                Emri = puntori.Emri,
                Mbiemri = puntori.Mbiemri,
                Pozita = puntori.Pozita,
                FabrikaId = puntori.FabrikaId,
                FabrikaEmri = puntori.Fabrika?.EmriFabrikes
            };

            return Ok(response);
        }

        [HttpGet("by-fabrika/{fabrikaId}")]
        public async Task<IActionResult> GetPuntoriByFabrika(int fabrikaId)
        {
            var puntori = await _context.Puntori
                .Include(p => p.Fabrika)
                .Where(p => p.FabrikaId == fabrikaId)
                .ToListAsync();

            var response = puntori.Select(p => new PuntoriResponseDto
            {
                Id = p.Id,
                Emri = p.Emri,
                Mbiemri = p.Mbiemri,
                Pozita = p.Pozita,
                FabrikaId = p.FabrikaId,
                FabrikaEmri = p.Fabrika?.EmriFabrikes
            });

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePuntori(PuntoriCreateDto dto)
        {
            // Verify that the fabrika exists
            var fabrika = await _context.Fabrika.FindAsync(dto.FabrikaId);
            if (fabrika == null) return BadRequest("Fabrika not found");

            var puntori = new Puntori
            {
                Emri = dto.Emri,
                Mbiemri = dto.Mbiemri,
                Pozita = dto.Pozita,
                FabrikaId = dto.FabrikaId
            };

            _context.Puntori.Add(puntori);
            await _context.SaveChangesAsync();

            var response = new PuntoriResponseDto
            {
                Id = puntori.Id,
                Emri = puntori.Emri,
                Mbiemri = puntori.Mbiemri,
                Pozita = puntori.Pozita,
                FabrikaId = puntori.FabrikaId,
                FabrikaEmri = fabrika.EmriFabrikes
            };

            return CreatedAtAction(nameof(GetPuntori), new { id = puntori.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePuntori(int id, PuntoriUpdateDto dto)
        {
            var puntori = await _context.Puntori.FindAsync(id);
            if (puntori == null) return NotFound();

            // Verify that the fabrika exists
            var fabrika = await _context.Fabrika.FindAsync(dto.FabrikaId);
            if (fabrika == null) return BadRequest("Fabrika not found");

            puntori.Emri = dto.Emri;
            puntori.Mbiemri = dto.Mbiemri;
            puntori.Pozita = dto.Pozita;
            puntori.FabrikaId = dto.FabrikaId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePuntori(int id)
        {
            var puntori = await _context.Puntori.FindAsync(id);
            if (puntori == null) return NotFound();

            _context.Puntori.Remove(puntori);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedPuntori()
        {
            // Check if puntori already exist
            if (await _context.Puntori.AnyAsync())
            {
                return Ok(new { message = "Puntori already exist" });
            }

            // Get existing fabrika or create them if they don't exist
            var fabrika = await _context.Fabrika.ToListAsync();
            if (!fabrika.Any())
            {
                // Create sample fabrika first
                var newFabrika = new[]
                {
                    new Fabrika { EmriFabrikes = "Fabrika e Ushqimit", AdresaFabrikes = "Rruga e Dëshmorëve, Prishtinë" },
                    new Fabrika { EmriFabrikes = "Fabrika e Pijeve", AdresaFabrikes = "Rruga e Pejës, Pejë" },
                    new Fabrika { EmriFabrikes = "Fabrika e Ëmbëlsirave", AdresaFabrikes = "Rruga e Gjakovës, Gjakovë" }
                };

                _context.Fabrika.AddRange(newFabrika);
                await _context.SaveChangesAsync();
                fabrika = newFabrika.ToList();
            }

            // Create sample puntori
            var puntori = new[]
            {
                new Puntori { Emri = "Arben", Mbiemri = "Krasniqi", Pozita = "Menaxher", FabrikaId = fabrika[0].Id },
                new Puntori { Emri = "Driton", Mbiemri = "Berisha", Pozita = "Inxhinier", FabrikaId = fabrika[0].Id },
                new Puntori { Emri = "Fisnik", Mbiemri = "Gashi", Pozita = "Teknik", FabrikaId = fabrika[1].Id },
                new Puntori { Emri = "Gent", Mbiemri = "Hoxha", Pozita = "Operator", FabrikaId = fabrika[1].Id },
                new Puntori { Emri = "Hysen", Mbiemri = "Ibrahimi", Pozita = "Kontrollor", FabrikaId = fabrika[2].Id },
                new Puntori { Emri = "Ilir", Mbiemri = "Jashari", Pozita = "Supervizor", FabrikaId = fabrika[2].Id }
            };

            _context.Puntori.AddRange(puntori);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sample puntori created successfully" });
        }
    }
}
