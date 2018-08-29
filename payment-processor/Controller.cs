using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_processor.Contracts;
using payment_processor.Models;
using StackExchange.Redis;

namespace payment_processor
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IConnectionMultiplexer _cacheConnection;

        public Controller(IConnectionMultiplexer cacheConnection)
        {
            this._cacheConnection = cacheConnection;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<GetPaymentStatusResponse>> GetPaymentStatus(string paymentId)
        {
            // get payment from cache
            var cache = this._cacheConnection.GetDatabase();
            RedisValue cachedValue = await cache.StringGetAsync(paymentId);

            if (cachedValue.IsNullOrEmpty) return new GetPaymentStatusResponse(paymentId, 0, "Not found");

            var payment = JsonConvert.DeserializeObject<Payment>(cachedValue);
            
            return new GetPaymentStatusResponse(payment.Id, payment.Amount, payment.Status);
        }

        [HttpPost("")]
        public void CreatePaymentSync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("")]
        public void UpdatePaymentStatus([FromBody] string payment)
        {
            throw new NotImplementedException();
        }
    }
}
