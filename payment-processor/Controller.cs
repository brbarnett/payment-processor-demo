using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_processor.Contracts;
using payment_processor.Contracts.External;
using payment_processor.Models;
using StackExchange.Redis;

namespace payment_processor
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private IConnectionMultiplexer _cacheConnection;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public Controller(IConnectionMultiplexer cacheConnection, IPaymentGatewayService paymentGatewayService)
        {
            this._cacheConnection = cacheConnection;
            this._paymentGatewayService = paymentGatewayService;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<GetPaymentStatusResponse>> GetPaymentStatus(string paymentId)
        {
            // get payment from cache
            var cache = this._cacheConnection.GetDatabase();
            RedisValue cachedValue = await cache.StringGetAsync($"payment.{paymentId}");

            if (cachedValue.IsNullOrEmpty) return new GetPaymentStatusResponse(paymentId, "Not found");

            var payment = JsonConvert.DeserializeObject<Payment>(cachedValue);

            return new GetPaymentStatusResponse(payment.Id, payment.Status);
        }

        [HttpPost("")]
        public async Task<ActionResult<SubmitPaymentResponse>> CreatePaymentSync([FromBody] SubmitPaymentRequest paymentRequest)
        {
            Payment payment = await this._paymentGatewayService.Process(paymentRequest);
            
            return new SubmitPaymentResponse(payment.Id, payment.Status);
        }
    }
}
