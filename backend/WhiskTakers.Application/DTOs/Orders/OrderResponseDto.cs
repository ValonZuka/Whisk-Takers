namespace WhiskTakers.Application.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}