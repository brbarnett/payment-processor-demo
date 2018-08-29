using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using payment_processor.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace payment_processor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RegisterAsBrokerConsumer();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void RegisterAsBrokerConsumer()
        {
            IPaymentGatewayService paymentGatewayService = new PaymentGatewayService(
                cacheConnection: ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION")),
                httpClient: new HttpClient()
            );

            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            string paymentsExchange = "payments";
            channel.ExchangeDeclare(exchange: paymentsExchange, type: "topic");

            string createQueue = "create";
            channel.QueueDeclare(queue: createQueue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false);
            channel.QueueBind(queue: createQueue,
                              exchange: paymentsExchange,
                              routingKey: "payments.create");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var submitPaymentRequest = JsonConvert.DeserializeObject<SubmitPaymentRequest>(Encoding.UTF8.GetString(body));

                paymentGatewayService.Process(submitPaymentRequest);
            };
            channel.BasicConsume(queue: createQueue,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine($"Monitoring queue: {createQueue}");

        }
    }
}
