namespace WhiskTakers.Application.DTOs.Products
{
    public class ProductUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int CategoryId { get; set; }
    }
}