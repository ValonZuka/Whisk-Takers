namespace WhiskTakers.Application.DTOs.Orders
{
    public class OrderCreateDto
    {
        public int TableNumber { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}//