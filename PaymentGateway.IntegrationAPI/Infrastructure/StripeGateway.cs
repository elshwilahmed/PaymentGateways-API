using Azure.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.IntegrationAPI.Data;
using PaymentGateway.IntegrationAPI.DTOs;
using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;
using PaymentGateway.IntegrationAPI.Models;
using PaymentGateway.IntegrationAPI.Responces;
using PaymentGateway.IntegrationAPI.Settings;
using Stripe;
using Stripe.Checkout;

namespace PaymentGateway.IntegrationAPI.Infrastructure
{
    public class StripeGateway : IPaymentGateway
    {
        public string GatewayName => "stripe";

        public string Currency => "USD";

        private readonly IOptions<StripeSettings> _options;
        private readonly AppDbContext context;
        private readonly ILogger<StripeGateway> logger;

        public StripeGateway(IOptions<StripeSettings> options, AppDbContext context, ILogger<StripeGateway> logger)
        {
            _options = options;
            this.context = context;
            this.logger = logger;
        }

        public async Task<APIResponse<PaymentResultDTO>> CreatePaymentIntentionAsync(UnifiedPaymentRequestDTO unifiedPaymentRequest)
        {
            string myUniqueReference = Guid.NewGuid().ToString();
            var order = new Order
            {
                Amount = unifiedPaymentRequest.Amount,
                CreatedAt = DateTime.UtcNow,
                Currency = unifiedPaymentRequest.Currency,
                Status = OrderStatus.Pending,
                CustomerId = "cust_987321",
                GatewayOrderId = myUniqueReference
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var options = new SessionCreateOptions
            {
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
                Mode = "payment",
                ClientReferenceId = order.Id.ToString(),
                Metadata = new Dictionary<string, string> { { "GatewayOrderId", myUniqueReference } },

                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = unifiedPaymentRequest.Currency.ToLower(),
                            UnitAmount = (long)(unifiedPaymentRequest.Amount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Order #" + order.Id
                            }
                        },
                        Quantity = 1,
                    },
                },
            };

            var client = new StripeClient(_options.Value.SecretKey);
            var service = client.V1.Checkout.Sessions;
            Session session = await service.CreateAsync(options);


            return APIResponse<PaymentResultDTO>.Success(new PaymentResultDTO
            {
                GatewayName = "Stripe",
                PaymentUrl = session.Url
            }, "Stripe Session Created");
        }

        public async Task<APIResponse<string>> ProcessWebhookAsync(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            

            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _options.Value.WebhookSecret);

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                if (session != null)
                {
                    var orderId = session.ClientReferenceId;

                    var order = await context.Orders.FirstOrDefaultAsync(o => o.Id.ToString() == orderId);

                    if (order != null && order.Status != OrderStatus.Paid)
                    {
                        order.Status = OrderStatus.Paid;

                        var transaction = new PaymentTransaction
                        {
                            Order = order,
                            GatewayName = "Stripe",
                            GatewayTransactionId = session.PaymentIntentId,
                            IsSuccess = true
                        };

                        context.PaymentTransactions.Add(transaction);
                        await context.SaveChangesAsync();

                        logger.LogInformation($"Order {order.Id} Paid Successfully via Stripe!");
                    }
                }
            }
            else
            {
                logger.LogInformation($"Received Unhandled Stripe Event: {stripeEvent.Type}");
            }

            return APIResponse<string>.Success("Webhook processed successfully");
        }
    }
}
