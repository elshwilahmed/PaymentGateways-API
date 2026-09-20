using System.Text.Json.Serialization;

namespace PaymentGateway.IntegrationAPI.DTOs
{
    public class PaymobWebhookDTO
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("obj")]
        public PaymobTransactionObjDTO Obj { get; set; }
    }

    public class PaymobTransactionObjDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("order")]
        public PaymobOrderDTO Order { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("error_occured")]
        public bool ErrorOccured { get; set; } 

        [JsonPropertyName("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonPropertyName("integration_id")]
        public int IntegrationId { get; set; }

        [JsonPropertyName("is_3d_secure")]
        public bool Is3dSecure { get; set; }

        [JsonPropertyName("is_auth")]
        public bool IsAuth { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_capture")]
        public bool IsCapture { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("source_data")]
        public PaymoSourceDataDTO SourceData { get; set; }

        [JsonPropertyName("owner")]
        public int Owner { get; set; }
    }

    public class PaymobOrderDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class PaymoSourceDataDTO
    {
        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("sub_type")]
        public string SubType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}