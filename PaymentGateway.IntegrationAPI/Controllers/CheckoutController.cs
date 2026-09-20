using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.IntegrationAPI.Data;
using PaymentGateway.IntegrationAPI.DTOs;
using PaymentGateway.IntegrationAPI.Infrastructure.interfaces;
using PaymentGateway.IntegrationAPI.Models;
using PaymentGateway.IntegrationAPI.Responces;

namespace PaymentGateway.IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IPaymentGatewayFactory _gatewayFactory;

        public CheckoutController(IPaymentGatewayFactory gatewayFactory, AppDbContext context)
        {
            _gatewayFactory = gatewayFactory;
            this.context = context;
        }


        [HttpPost("")]
        public async Task<ActionResult<APIResponse<PaymentResultDTO>>> CreatePayment([FromBody] UnifiedPaymentRequestDTO request)
        {
            try
            {
                var gateway = _gatewayFactory.GetGatewayByCurrency(request.Currency);

                var result = await gateway.CreatePaymentIntentionAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(APIResponse<PaymentResultDTO>.Failure(ex.Message));
            }
        }   
    }
}

