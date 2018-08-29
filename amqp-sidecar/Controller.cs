using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

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
        public ActionResult<string> EnqueueMessage([FromBody] object messageBody)
        {
            string exchange = Request.Headers["amqp-exchange"];
            if(String.IsNullOrEmpty(exchange)) return BadRequest("Could not find required `amqp-exchange` header.");

            string routingKey = Request.Headers["amqp-routing-key"];
            if(String.IsNullOrEmpty(exchange)) return BadRequest("Could not find required `amqp-routing-key` header.");

            // enqueue request as message
            using (var channel = this._brokerConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange, type: "topic");

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageBody));
                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }

            return Ok();
        }
    }
}
