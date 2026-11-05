using Microsoft.AspNetCore.Mvc;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using WhiskTakers.Application.DTOs.Orders;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
                PreparationStartedAtUtc = o.PreparationStartedAtUtc,
                TimerExtensionMinutes = o.TimerExtensionMinutes,
                WaiterName = o.WaiterName ?? "Unknown",
                WaiterEmail = o.WaiterEmail ?? "",
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    ProductPrice = i.UnitPriceAtOrder > 0 ? i.UnitPriceAtOrder : i.Product?.Price ?? 0,
                    UnitCostAtOrder = i.UnitCostAtOrder,
                    PrepMinutes = i.PrepMinutesAtOrder,
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
                PreparationStartedAtUtc = order.PreparationStartedAtUtc,
                TimerExtensionMinutes = order.TimerExtensionMinutes,
                WaiterName = order.WaiterName ?? "Unknown",
                WaiterEmail = order.WaiterEmail ?? "",
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    ProductPrice = i.UnitPriceAtOrder > 0 ? i.UnitPriceAtOrder : i.Product?.Price ?? 0,
                    UnitCostAtOrder = i.UnitCostAtOrder,
                    PrepMinutes = i.PrepMinutesAtOrder,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Order must have at least one item.");

            var waiterName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var waiterEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (dto.Items.Any(item => item.Quantity < 1)) return BadRequest("Item quantities must be at least one.");
            var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToArray();
            var products = await _context.Products.Where(product => productIds.Contains(product.Id) && product.IsAvailable).ToDictionaryAsync(product => product.Id);
            if (productIds.Any(productId => !products.ContainsKey(productId))) return BadRequest("One or more products do not exist.");

            
            var order = new Order
            {
                TableNumber = dto.TableNumber,
                WaiterName = waiterName,
                WaiterEmail = waiterEmail,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitCostAtOrder = products[i.ProductId].UnitCost,
                    UnitPriceAtOrder = products[i.ProductId].Price,
                    PrepMinutesAtOrder = products[i.ProductId].PrepMinutes
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
                PreparationStartedAtUtc = order.PreparationStartedAtUtc,
                TimerExtensionMinutes = order.TimerExtensionMinutes,
                WaiterName = waiterName ?? "Unknown",  // Use the extracted waiter name from JWT
                WaiterEmail = waiterEmail ?? "",  // Use the extracted waiter email from JWT
                Items = order.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = _context.Products.FirstOrDefault(p => p.Id == i.ProductId)?.Name ?? "",
                    ProductPrice = i.UnitPriceAtOrder,
                    UnitCostAtOrder = i.UnitCostAtOrder,
                    PrepMinutes = i.PrepMinutesAtOrder,
                    Quantity = i.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderCreateDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0 || dto.Items.Any(item => item.Quantity < 1))
                return BadRequest("Order must contain valid items.");
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToArray();
            var products = await _context.Products.Where(product => productIds.Contains(product.Id) && product.IsAvailable).ToDictionaryAsync(product => product.Id);
            if (productIds.Any(productId => !products.ContainsKey(productId))) return BadRequest("One or more products do not exist.");

            order.TableNumber = dto.TableNumber;

            order.Items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCostAtOrder = products[i.ProductId].UnitCost,
                UnitPriceAtOrder = products[i.ProductId].Price,
                PrepMinutesAtOrder = products[i.ProductId].PrepMinutes
            }).ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "chef,admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (!new[] { "Pending", "Preparing", "Ready", "Done", "Canceled" }.Contains(request.Status))
                return BadRequest("Invalid status");

            order.Status = request.Status;
            if (request.Status == "Preparing")
            {
                order.PreparationStartedAtUtc = DateTime.UtcNow;
                order.TimerExtensionMinutes = 0;
            }
            await _context.SaveChangesAsync();

            return Ok(new { preparationStartedAtUtc = order.PreparationStartedAtUtc, timerExtensionMinutes = order.TimerExtensionMinutes });
        }

        [HttpPost("{id}/timer-extension")]
        [Authorize(Roles = "chef,admin")]
        public async Task<IActionResult> ExtendPreparationTimer(int id, [FromBody] TimerExtensionRequest request)
        {
            if (request.Minutes is not 5 and not 10) return BadRequest("Only 5 or 10 minute extensions are supported.");
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status != "Preparing" || order.PreparationStartedAtUtc == null)
                return Conflict("Start the preparation timer before extending it.");

            order.TimerExtensionMinutes += request.Minutes;
            await _context.SaveChangesAsync();
            return Ok(new { timerExtensionMinutes = order.TimerExtensionMinutes });
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

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class TimerExtensionRequest
    {
        public int Minutes { get; set; }
    }
}
