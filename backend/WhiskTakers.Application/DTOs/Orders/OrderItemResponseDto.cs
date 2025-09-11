namespace WhiskTakers.Application.DTOs.Orders
{
    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}