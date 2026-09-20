using System.Text.Json.Serialization;

namespace PaymentGateway.IntegrationAPI.DTOs
{
    public class PaymobIntentionResponseDTO
    {
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = null!;

        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
    }
}
