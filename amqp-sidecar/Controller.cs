using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace amqp_sidecar.Controllers
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

        [HttpPost("")]
        public async Task<ActionResult<string>> CreatePaymentSync([FromBody] SubmitPaymentRequest paymentRequest)
        {
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
        }
    }
}
