using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using payment_api.Contracts;
using payment_api.Contracts.External;

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
            if (!serviceResponse.IsSuccessStatusCode) return new GetPaymentStatusResponse(String.Empty, "Not found");

            PaymentProcessorGetPaymentStatusResponse paymentProcessorResponse = await serviceResponse.Content.ReadAsAsync<PaymentProcessorGetPaymentStatusResponse>();;
            return new GetPaymentStatusResponse(paymentProcessorResponse.PaymentId, paymentProcessorResponse.PaymentStatus);
        }

        [HttpPost("submitSync")]
        public async Task<ActionResult<SubmitPaymentResponse>> CreatePaymentSync([FromBody] SubmitPaymentRequest paymentRequest)
        {
            PaymentProcessorSubmitPaymentRequest paymentProcessorRequest = new PaymentProcessorSubmitPaymentRequest(paymentRequest.AccountNumber, paymentRequest.PaymentAmount);
            var serviceResponse = await this._httpClient.PostAsJsonAsync($"http://payment-processor", paymentProcessorRequest);
            if (!serviceResponse.IsSuccessStatusCode) return new SubmitPaymentResponse(String.Empty, "Error");

            PaymentProcessorSubmitPaymentResponse paymentProcessorResponse = await serviceResponse.Content.ReadAsAsync<PaymentProcessorSubmitPaymentResponse>();
            return new SubmitPaymentResponse(paymentProcessorResponse.PaymentId, paymentProcessorResponse.PaymentStatus);
        }

        [HttpPost("submitAsync")]
        public async Task<ActionResult<SubmitPaymentResponse>> CreatePaymentAsync([FromBody] SubmitPaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
