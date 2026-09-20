using System.Text.Json.Serialization;

namespace PaymentGateway.IntegrationAPI.DTOs
{
    public class PaymentResultDTO
    {
        public string OrderId { get; set; }
        public string GatewayName { get; set; }
        public string PaymentUrl { get; set; }
    }
}
