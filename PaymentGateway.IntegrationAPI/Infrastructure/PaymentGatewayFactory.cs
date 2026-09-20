using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;

namespace PaymentGateway.IntegrationAPI.Infrastructure
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly ILogger<PaymentGatewayFactory> logger;

        public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways, ILogger<PaymentGatewayFactory> logger)
        {
            _gateways = gateways;
            this.logger = logger;
        }

        public IPaymentGateway GetGatewayByCurrency(string currency)
        {
            IPaymentGateway gateway = null;

            if (string.Equals(currency, "EGP", StringComparison.OrdinalIgnoreCase))
            {
                gateway = _gateways.FirstOrDefault(g =>
                g.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase));
            }
            else if (string.Equals(currency, "USD", StringComparison.OrdinalIgnoreCase))
            {
                gateway = _gateways.FirstOrDefault(g => 
                g.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new ArgumentException($"Currency '{currency}' is not supported.");
            }

            if (gateway == null)
            {
                throw new ArgumentException($"Gateway for currency '{currency}' is not registered in the system.");
            }

            return gateway;
        }

        public IPaymentGateway GetGateway(string gatewayName)
        {
            var gateway = _gateways.FirstOrDefault(g =>
                g.GatewayName.Equals(gatewayName, StringComparison.OrdinalIgnoreCase));

            if (gateway == null)
            {
                logger.LogError($"Gateway Name '{gatewayName}' Not Found");
                throw new ArgumentException($"Payment gateway '{gatewayName}' is not supported.");
            }

            return gateway;
        }
    }
}