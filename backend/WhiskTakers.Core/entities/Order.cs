namespace WhiskTakers.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int TableNumber { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; 

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } 
        public int Quantity { get; set; }
    }
}
