namespace WhiskTakers.Application.DTOs.Products
{
    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public double UnitCost { get; set; }
        public int PrepMinutes { get; set; } = 10;
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
