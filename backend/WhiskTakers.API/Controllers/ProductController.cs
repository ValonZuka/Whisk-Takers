using Microsoft.AspNetCore.Mvc;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = new[]
            {
                new { Id = 1, Name = "Pizza Margherita", Price = 10.0 },
                new { Id = 2, Name = "Classic Burger", Price = 7.5 },
                new { Id = 3, Name = "Pasta Carbonara", Price = 8.0 },
            };

            return Ok(products);
        }
    }
}
