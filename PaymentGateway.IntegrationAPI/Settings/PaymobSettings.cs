namespace PaymentGateway.IntegrationAPI.Settings
{
    public class PaymobSettings
    {
        public string BaseUrl { get; set; }
        public string secretKey { get; set; }
        public string publicKey { get; set; }
        public int integrationID { get; set; }
        public string HMAC { get; set; }
    }
}
