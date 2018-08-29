using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Contracts;

namespace payment_api
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public Controller(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<GetPaymentStatusResponse>> GetPaymentStatus(string paymentId)
        {
            var serviceResponse = await this._httpClient.GetAsync($"http://payment-processor/{paymentId}");

            GetPaymentStatusResponse response = null;
            if (serviceResponse.IsSuccessStatusCode)
            {
                response = await serviceResponse.Content.ReadAsAsync<GetPaymentStatusResponse>();
            }

            return response;
        }

        [HttpPost("submitSync")]
        public void CreatePaymentSync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }

        [HttpPost("submitAsync")]
        public void CreatePaymentAsync([FromBody] string payment)
        {
            throw new NotImplementedException();
        }
    }
}
