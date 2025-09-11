using Microsoft.AspNetCore.Mvc;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using WhiskTakers.Application.DTOs.Orders;
using Microsoft.EntityFrameworkCore;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly WhiskTakersDbContext _context;

        public OrdersController(WhiskTakersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            var response = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                TableNumber = o.TableNumber,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    ProductPrice = i.Product?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                TableNumber = order.TableNumber,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    ProductPrice = i.Product?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Order must have at least one item.");

            var order = new Order
            {
                TableNumber = dto.TableNumber,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                TableNumber = order.TableNumber,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = _context.Products.FirstOrDefault(p => p.Id == i.ProductId)?.Name ?? "",
                    ProductPrice = _context.Products.FirstOrDefault(p => p.Id == i.ProductId)?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderCreateDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            order.TableNumber = dto.TableNumber;

            order.Items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteOrder(int id)
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null) return NotFound();

                // Fshi items manualisht
                _context.OrderItems.RemoveRange(order.Items);

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return NoContent();
            }

    }
}
