namespace PaymentGateway.IntegrationAPI.Models
{
    public enum OrderStatus
    {
        None = 1,
        Pending,
        Paid,
        Failed,
        Refunded
    }
    public class Order
    {
        public int Id { get; set; }

        public string CustomerId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public OrderStatus Status { get; set; } 
 
        public string? GatewayOrderId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    }
}

