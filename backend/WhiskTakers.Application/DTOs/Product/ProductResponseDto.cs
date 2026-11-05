namespace WhiskTakers.Application.DTOs.Products
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public double UnitCost { get; set; }
        public int PrepMinutes { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }
}
