using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_api.Contracts;
using payment_api.Contracts.External;
using RabbitMQ.Client;

namespace payment_api
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IConnection _brokerConnection;
        private readonly HttpClient _httpClient;

        public Controller(IConnection brokerConnection, HttpClient httpClient)
        {
            this._brokerConnection = brokerConnection;
            this._httpClient = httpClient;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<GetPaymentStatusResponse>> GetPaymentStatus(string paymentId)
        {
            var serviceResponse = await this._httpClient.GetAsync($"http://payment-processor/{paymentId}");
            if (!serviceResponse.IsSuccessStatusCode) return new GetPaymentStatusResponse(String.Empty, "Not found");

            PaymentProcessorGetPaymentStatusResponse paymentProcessorResponse = await serviceResponse.Content.ReadAsAsync<PaymentProcessorGetPaymentStatusResponse>(); ;
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
        public ActionResult<SubmitPaymentResponse> CreatePaymentAsync([FromBody] SubmitPaymentRequest paymentRequest)
        {
            PaymentProcessorSubmitPaymentRequest paymentProcessorRequest = new PaymentProcessorSubmitPaymentRequest(paymentRequest.AccountNumber, paymentRequest.PaymentAmount);

            // enqueue request as message
            using (var channel = this._brokerConnection.CreateModel())
            {
                string paymentsExchange = "payments";
                channel.ExchangeDeclare(exchange: paymentsExchange, type: "topic");

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(paymentProcessorRequest));
                channel.BasicPublish(exchange: paymentsExchange,
                                     routingKey: "payments.create",
                                     basicProperties: null,
                                     body: body);
            }

            return new SubmitPaymentResponse(paymentProcessorRequest.PaymentId, "Pending");
        }
    }
}
