using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.IntegrationAPI.Data;
using PaymentGateway.IntegrationAPI.DTOs;
using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;
using PaymentGateway.IntegrationAPI.Models;
using PaymentGateway.IntegrationAPI.Responces;
using PaymentGateway.IntegrationAPI.Settings;

namespace PaymentGateway.IntegrationAPI.Infrastructure
{
    public class PaymobGateway : IPaymentGateway
    {
        public string GatewayName => "paymob";

        public string Currency => "EGP";

        private readonly HttpClient client;
        private readonly IOptions<PaymobSettings> options;
        private readonly AppDbContext context;
        private readonly ILogger<PaymobGateway> logger;

        public PaymobGateway(HttpClient client, IOptions<PaymobSettings> options, 
                      AppDbContext context, ILogger<PaymobGateway> logger)
        {
            this.client = client;
            this.options = options;
            this.context = context;
            this.logger = logger;

            client.BaseAddress = new Uri(options.Value.BaseUrl);
        }
        public async Task<APIResponse<PaymentResultDTO>> CreatePaymentIntentionAsync(UnifiedPaymentRequestDTO paymobRequest)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Token {options.Value.secretKey}");

            string myUniqueReference = Guid.NewGuid().ToString();
            var request = new PaymobRequestDTO
            {
                Amount = paymobRequest.Amount,
                Currency =paymobRequest.Currency,
                CustomerID = "cust_123789",
                PaymentMethods = new List<int> { options.Value.integrationID},
                BillingData = new PaymobBillingDataDTO
                {
                    FirstName = paymobRequest.FirstName,
                    LastName = paymobRequest.LastName,
                    Email = paymobRequest.Email,
                    PhoneNumber = paymobRequest.PhoneNumber,

                    Apartment = paymobRequest.Apartment ?? "NA",
                    Floor = paymobRequest.Floor ?? "NA",
                    Street = paymobRequest.Street ?? "NA",
                    Building = paymobRequest.Building ?? "NA",

                    City = paymobRequest.City ?? "NA",
                    Country = paymobRequest.Country ?? "NA"
                },
                SpecialReference = myUniqueReference
            };



            var response = await client.PostAsJsonAsync("/v1/intention/", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PaymobIntentionResponseDTO>();

                var order = new Order
                {
                    Amount = paymobRequest.Amount,
                    CreatedAt = DateTime.UtcNow,
                    Currency = request.Currency,
                    Status = OrderStatus.Pending,
                    CustomerId = request.CustomerID,
                    GatewayOrderId = myUniqueReference
                };
                context.Orders.Add(order);
                await context.SaveChangesAsync();

                string paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={options.Value.publicKey}&clientSecret={result!.ClientSecret}";

                var resultDto = new PaymentResultDTO
                {
                    OrderId = order.Id.ToString(),
                    GatewayName = "Paymob",
                    PaymentUrl = paymentUrl
                };

                return APIResponse<PaymentResultDTO>.Success(resultDto, "Payment intent created successfully");  // return url
            }

            var error = await response.Content.ReadAsStringAsync();

            logger.LogError(error);
            return APIResponse<PaymentResultDTO>.Failure($"Error Detailes: {error}");

        }

        // To verify HMAC 
        bool VerifyPaymobHmac(JsonElement payload, string receivedHmac)
        {
            var obj = payload.GetProperty("obj");

            var sb = new StringBuilder();
            sb.Append(obj.GetProperty("amount_cents").GetInt32());
            sb.Append(obj.GetProperty("created_at").GetString());
            sb.Append(obj.GetProperty("currency").GetString());
            sb.Append(obj.GetProperty("error_occured").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("has_parent_transaction").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("id").GetInt32());
            sb.Append(obj.GetProperty("integration_id").GetInt32());
            sb.Append(obj.GetProperty("is_3d_secure").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("is_auth").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("is_capture").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("is_refunded").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("is_standalone_payment").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("is_voided").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("order").GetProperty("id").GetInt32());
            sb.Append(obj.GetProperty("owner").GetInt32());
            sb.Append(obj.GetProperty("pending").GetBoolean().ToString().ToLower());
            sb.Append(obj.GetProperty("source_data").GetProperty("pan").GetString());
            sb.Append(obj.GetProperty("source_data").GetProperty("sub_type").GetString());
            sb.Append(obj.GetProperty("source_data").GetProperty("type").GetString());
            sb.Append(obj.GetProperty("success").GetBoolean().ToString().ToLower());

            string concatenatedString = sb.ToString();

            var keyByte = Encoding.UTF8.GetBytes(options.Value.HMAC);
            var messageBytes = Encoding.UTF8.GetBytes(concatenatedString);

            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                var hashMessage = hmacsha512.ComputeHash(messageBytes);
                var generatedHmac = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();

                return generatedHmac == receivedHmac.ToLower();
            }
        }

        public async Task<APIResponse<string>> ProcessWebhookAsync(HttpRequest request)
        {
            var hmac = request.Query["hmac"].ToString();

            if (request.ContentLength == null || request.ContentLength == 0)
            {
                logger.LogInformation("Webhook received with empty body ( posiblemente Test/Ping from Paymob). Ignored.");
                return APIResponse<string>.Success("Ping received");
            }

            using var document = await JsonDocument.ParseAsync(request.Body);
            var payload = document.RootElement;

            if (!VerifyPaymobHmac(payload, hmac))
            {
                logger.LogWarning("Unauthorized access attempt! Invalid HMAC received. Payload: {Payload}", payload.ToString());
                return APIResponse<string>.Failure("Unauthorized");
            }

            var obj = payload.GetProperty("obj");
            string gatewayTransactionId = obj.GetProperty("id").GetInt32().ToString();
            string gatewayOrderId = obj.GetProperty("order").GetProperty("id").GetInt32().ToString();
            bool isSuccess = obj.GetProperty("success").GetBoolean();

            if (payload.GetProperty("type").GetString() != "TRANSACTION")
            {
                logger.LogInformation("GatewayTransactionId {TransactionId} was already processed. Ignored.", gatewayTransactionId);
                return APIResponse<string>.Failure("Ignored: Not a transaction");
            }

            // Idempotency Safety  (double click)
            bool isAlreadyProcessed = await context.PaymentTransactions
                .AnyAsync(t => t.GatewayTransactionId == gatewayTransactionId);

            if (isAlreadyProcessed)
            {
                logger.LogInformation($"Order {gatewayOrderId} Processed Successfully.TransactionId: {gatewayTransactionId}");
                return APIResponse<string>.Success("Already processed");
            }

            var orderNode = obj.GetProperty("order");

            string mySpecialReference = null;
            if (orderNode.TryGetProperty("merchant_order_id", out var merchantOrderIdProp) && merchantOrderIdProp.ValueKind != JsonValueKind.Null)
            {
                mySpecialReference = merchantOrderIdProp.GetString();
            }

            if (string.IsNullOrEmpty(mySpecialReference))
            {
                logger.LogInformation("Notification received without merchant_order_id. Ignored.");
                return APIResponse<string>.Success("Ignored: No merchant order id");
            }

            var order = await context.Orders.FirstOrDefaultAsync(o => o.GatewayOrderId == mySpecialReference);
            if (order != null)
            {
                order.Status = isSuccess ? OrderStatus.Paid : OrderStatus.Failed;

                var transaction = new PaymentTransaction
                {
                    Order = order,
                    GatewayName = "Paymob",
                    GatewayTransactionId = gatewayTransactionId,
                    IsSuccess = isSuccess,
                    ErrorMessage = isSuccess ? null : obj.GetProperty("data").GetProperty("message").GetString()
                };

                context.PaymentTransactions.Add(transaction);
                await context.SaveChangesAsync();

                return APIResponse<string>.Success($"Order {order.Id} Paid Successfully!");
            }
            logger.LogError($" Webhook received for GatewayOrderId {gatewayOrderId} but order was not found in DB!");
            return APIResponse<string>.Failure("not found");
        }
    }
}
