using System.Text.Json.Serialization;

namespace PaymentGateway.IntegrationAPI.DTOs
{
    public class PaymobRequestDTO
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        public string CustomerID { get; set; }

        [JsonPropertyName("payment_methods")]
        public List<int> PaymentMethods { get; set; }

        [JsonPropertyName("billing_data")]
        public PaymobBillingDataDTO BillingData { get; set; }

        [JsonPropertyName("special_reference")]
        public string? SpecialReference { get; set; }
    }

    public class PaymobBillingDataDTO
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonIgnore]
        [JsonPropertyName("apartment")]
        public string? Apartment { get; set; }

        [JsonPropertyName("floor")]
        public string? Floor { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("building")]
        public string? Building { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}

