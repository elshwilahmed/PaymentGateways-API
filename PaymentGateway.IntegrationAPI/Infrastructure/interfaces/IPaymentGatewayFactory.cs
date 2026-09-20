using PaymentGateway.IntegrationAPI.Responces;

namespace PaymentGateway.IntegrationAPI.Infrastructure.interfaces
{
    public interface IPaymentGatewayFactory
    {
        public IPaymentGateway GetGatewayByCurrency(string currency);
        public IPaymentGateway GetGateway(string gatewayName);
    }
}
