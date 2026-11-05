using Microsoft.AspNetCore.Mvc;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using WhiskTakers.Application.DTOs.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly WhiskTakersDbContext _context;

        public ProductsController(WhiskTakersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var query = _context.Products.AsQueryable();
            if (!User.IsInRole("admin") && !User.IsInRole("chef"))
                query = query.Where(product => product.IsAvailable);
            var products = await query
                .Include(p => p.Category)
                .ToListAsync();

            var response = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                UnitCost = p.UnitCost,
                PrepMinutes = p.PrepMinutes,
                CategoryName = p.Category?.Name ?? "",
                IsAvailable = p.IsAvailable
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || (!product.IsAvailable && !User.IsInRole("admin") && !User.IsInRole("chef"))) return NotFound();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                UnitCost = product.UnitCost,
                PrepMinutes = product.PrepMinutes,
                CategoryName = product.Category?.Name ?? "",
                IsAvailable = product.IsAvailable
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin,chef")]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0 || dto.UnitCost < 0 || dto.PrepMinutes is < 1 or > 240 || !await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                return BadRequest("A name, positive price, valid cost, preparation time, and category are required.");
            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty,
                Price = dto.Price,
                UnitCost = dto.UnitCost,
                PrepMinutes = dto.PrepMinutes,
                CategoryId = dto.CategoryId,
                IsAvailable = dto.IsAvailable
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var response = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                UnitCost = product.UnitCost,
                PrepMinutes = product.PrepMinutes,
                CategoryName = _context.Categories.FirstOrDefault(c => c.Id == dto.CategoryId)?.Name ?? "",
                IsAvailable = product.IsAvailable
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,chef")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Price <= 0 || dto.UnitCost < 0 || dto.PrepMinutes is < 1 or > 240 || !await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                return BadRequest("A name, positive price, valid cost, preparation time, and category are required.");
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name.Trim();
            product.Description = dto.Description?.Trim() ?? string.Empty;
            product.ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty;
            product.Price = dto.Price;
            product.UnitCost = dto.UnitCost;
            product.PrepMinutes = dto.PrepMinutes;
            product.CategoryId = dto.CategoryId; // përditëso Category
            if (dto.IsAvailable.HasValue) product.IsAvailable = dto.IsAvailable.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }
            

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,chef")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("seed")]
        [Authorize(Roles = "admin,chef")]
        public async Task<IActionResult> SeedProducts()
        {
            // Check if products already exist
            if (await _context.Products.AnyAsync())
            {
                return Ok(new { message = "Products already exist" });
            }

            // Create some sample categories first
            var categories = new[]
            {
                new Category { Name = "Starters" },
                new Category { Name = "Mains" },
                new Category { Name = "Desserts" }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Create sample products
            var products = new[]
            {
                new Product { Name = "Caesar Salad", Price = 12.99, CategoryId = categories[0].Id },
                new Product { Name = "Garlic Bread", Price = 8.99, CategoryId = categories[0].Id },
                new Product { Name = "Grilled Salmon", Price = 24.99, CategoryId = categories[1].Id },
                new Product { Name = "Beef Steak", Price = 28.99, CategoryId = categories[1].Id },
                new Product { Name = "Chicken Pasta", Price = 18.99, CategoryId = categories[1].Id },
                new Product { Name = "Chocolate Cake", Price = 9.99, CategoryId = categories[2].Id },
                new Product { Name = "Ice Cream", Price = 6.99, CategoryId = categories[2].Id }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sample products created successfully" });
        }

        [HttpPost("seed-menu")]
        [Authorize(Roles = "admin,chef")]
        public async Task<IActionResult> SeedMenu()
        {
            var menu = new[]
            {
                new MenuItem("Desserts", "Trileçe me Karamel", 2.50, "Pandispanjë e butë, qumësht i ëmbël dhe karamel i përgatitur në shtëpi."),
                new MenuItem("Desserts", "Cheesecake me Boronicë", 3.50, "Bazë biskote, krem djathi i lehtë dhe salcë me boronica të egra."),
                new MenuItem("Desserts", "Tortë Whisk-Takers me Çokollatë", 4.00, "Tri shtresa pandispanje me çokollatë të zezë belge dhe fëstëk të thërrmuar."),
                new MenuItem("Desserts", "Soufflé Çokollate me Akullore", 3.00, "Soufflé i ngrohtë me zemër të shkrirë çokollate, shërbyer me akullore vanilje."),
                new MenuItem("Desserts", "Tiramisu Klasike", 3.20, "Biskota savoiardi me kafe espresso, krem mascarpone dhe kakao."),
                new MenuItem("Desserts", "Tartë me Fruta Mali", 2.80, "Brumë i shkrifët, krem vaniljeje dhe fruta mali të stinës."),
                new MenuItem("Drinks", "Espresso", 1.00, "Kafe espresso e sapobluar, me kremë të artë."),
                new MenuItem("Drinks", "Macchiato e Madhe", 1.50, "Espresso e dyfishtë me qumësht të shkumëzuar."),
                new MenuItem("Drinks", "Cappuccino me Vanilje", 2.00, "Espresso, qumësht i shkumëzuar dhe aromë vaniljeje."),
                new MenuItem("Drinks", "Freddo Espresso", 2.20, "Espresso e ftohtë e tundur me akull dhe shkumë të lehtë."),
                new MenuItem("Drinks", "Çaj Mali me Limon", 1.20, "Çaj mali aromatik me limon të freskët."),
                new MenuItem("Drinks", "Limonatë Shtëpie me Mente", 2.50, "Lëng limoni i freskët, ujë i gazuar, shurup sheqeri dhe gjethe menteje."),
                new MenuItem("Drinks", "Smoothie me Dredhëz dhe Banane", 3.50, "Dredhëza, banane, qumësht bajameje dhe një lugë mjaltë."),
                new MenuItem("Breakfast & Appetizers", "Croissant me Gjalpë", 1.50, "Croissant i pjekur freskët me gjalpë."),
                new MenuItem("Breakfast & Appetizers", "Croissant me Nutella", 1.80, "Croissant i ngrohtë i mbushur me krem çokollate dhe lajthie."),
                new MenuItem("Breakfast & Appetizers", "Toast me Avokado dhe Vezë", 3.80, "Bukë integrale e thekur, avokado me limon, vezë poached dhe fara chia."),
                new MenuItem("Breakfast & Appetizers", "Sanduiç me Proshutë dhe Mozzarella", 3.00, "Baguetë e freskët me proshutë krudo, mozzarella, domate dhe pesto."),
                new MenuItem("Breakfast & Appetizers", "Pancakes me Shurup Maple", 3.50, "Tri pancakes të buta me gjalpë, shurup maple dhe fruta mali."),
                new MenuItem("Breakfast & Appetizers", "Petulla Tradicionale me Djathë", 2.50, "Petulla të sapopërgatitura, të shërbyera me djathë të bardhë."),
                new MenuItem("Bakery Specials", "Baguetë Franceze", 1.00, "Baguetë me kore krokante dhe brendësi të butë, e pjekur freskët."),
                new MenuItem("Bakery Specials", "Cinnamon Roll me Kanellë", 2.20, "Brumë i butë i mbështjellë me kanellë dhe glazurë të lehtë."),
                new MenuItem("Bakery Specials", "Cookie me Copa Çokollate", 1.20, "Biskotë e butë me gjalpë dhe copa çokollate."),
                new MenuItem("Bakery Specials", "Muffin me Çokollatë dhe Arra", 1.50, "Muffin me kakao, copa çokollate dhe arra të pjekura.")
            };

            var existingCategories = await _context.Categories.ToListAsync();
            foreach (var name in menu.Select(item => item.Category).Distinct())
            {
                if (!existingCategories.Any(category => category.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    var category = new Category { Name = name };
                    _context.Categories.Add(category);
                    existingCategories.Add(category);
                }
            }
            await _context.SaveChangesAsync();

            var existingProducts = await _context.Products.ToListAsync();
            var added = 0;
            foreach (var item in menu)
            {
                if (existingProducts.Any(product => product.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))) continue;
                var categoryId = existingCategories.First(category => category.Name.Equals(item.Category, StringComparison.OrdinalIgnoreCase)).Id;
                _context.Products.Add(new Product { Name = item.Name, Description = item.Description, Price = item.Price, PrepMinutes = item.PrepMinutes, CategoryId = categoryId });
                added++;
            }
            await _context.SaveChangesAsync();
            return Ok(new { added, total = menu.Length, message = $"Added {added} menu items; existing dishes were kept." });
        }

        private sealed record MenuItem(string Category, string Name, double Price, string Description, int PrepMinutes = 10);
    }
}
