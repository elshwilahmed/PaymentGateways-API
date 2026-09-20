using System.Text.Json;
using PaymentGateway.IntegrationAPI.DTOs;
using PaymentGateway.IntegrationAPI.Responces;

namespace PaymentGateway.IntegrationAPI.Infrastructure.interfaces
{
    public interface IPaymentGateway
    {
        string GatewayName { get; }
        string Currency { get; }

        Task<APIResponse<PaymentResultDTO>> CreatePaymentIntentionAsync(UnifiedPaymentRequestDTO unifiedPaymentRequest);
        Task<APIResponse<string>> ProcessWebhookAsync(HttpRequest request);

    }
}
