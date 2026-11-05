namespace WhiskTakers.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Price { get; set; }
        public double UnitCost { get; set; }
        public int PrepMinutes { get; set; } = 10;
        public bool IsAvailable { get; set; } = true;

        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
