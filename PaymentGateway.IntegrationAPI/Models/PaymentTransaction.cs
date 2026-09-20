namespace PaymentGateway.IntegrationAPI.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public string GatewayName { get; set; } 

        public string? GatewayTransactionId { get; set; }

        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
