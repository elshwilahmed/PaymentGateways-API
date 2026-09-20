using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.IntegrationAPI.Data;
using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;

namespace PaymentGateway.IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentGatewayFactory _gatewayFactory;

        public WebhookController(IPaymentGatewayFactory gatewayFactory, AppDbContext context)
        {
            _gatewayFactory = gatewayFactory;
        }


        // اللينك هيكون: POST api/webhook/paymob أو api/checkout/webhook/stripe
        [HttpPost("{gatewayName}")]
        public async Task<ActionResult> Webhook(string gatewayName)
        {
            try
            {
                var gateway = _gatewayFactory.GetGateway(gatewayName);

                var res = await gateway.ProcessWebhookAsync(Request);

                return Ok(res.Message ?? "Recieved");
            }
            catch (ArgumentException ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
