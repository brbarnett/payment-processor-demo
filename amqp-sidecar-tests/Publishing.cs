using System.Net.Http;

using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Fakes;

using System.Net;

namespace amqp_sidecar_tests
{
    // reference: https://github.com/Parametric/RabbitMQ.Fakes/blob/master/RabbitMQ.Fakes.Tests/UseCases/SendMessages.cs
    [TestFixture]
    public class Publishing
    {
        private RabbitServer _rabbitServer;
        private FakeConnectionFactory _connectionfactory;

        [SetUp]
        public void Setup()
        {
            // rabbitmq
            this._rabbitServer = new RabbitServer();
            this._connectionfactory = new FakeConnectionFactory(this._rabbitServer);
        }

        [Test, Sequential]
        public void PublishNullMessageBody(
            [Values("", "payments", "", "payments")] string exchange,
            [Values("", "", "payments.create", "payments.create")] string routingKey)
        {
            // arrange
            var controller = new amqp_sidecar.Controllers.Controller(this._connectionfactory.CreateConnection(), new HttpClient());

            // act
            ActionResult<string> actionResult = controller.EnqueueMessage(null, exchange, routingKey);

            // assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.That(result != null, "Result is not BadRequestObjectResult");
            Assert.That(result.StatusCode == (int)HttpStatusCode.BadRequest, "Result should be status code 400");
        }

        [Test]
        public void PublishMessageValid()
        {
            // arrange
            var controller = new amqp_sidecar.Controllers.Controller(this._connectionfactory.CreateConnection(), new HttpClient());
            var message = new SubmitPaymentRequest
            {
                AccountNumber = "12345",
                PaymentAmount = 100
            };

            // act
            ActionResult<string> actionResult = controller.EnqueueMessage(message, "payments", "payments.create");

            // assert
            var result = actionResult.Result as OkResult;
            Assert.That(result != null, "Result is not OkResult");
            Assert.That(result.StatusCode == (int)HttpStatusCode.OK, "Result should be status code 200");
            Assert.That(this._rabbitServer.Exchanges["payments"].Messages.Count, Is.EqualTo(1));
        }

        private class SubmitPaymentRequest
        {
            public string AccountNumber { get; set; }

            public decimal PaymentAmount { get; set; }
        }
    }
}