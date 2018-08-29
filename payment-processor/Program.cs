using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received:", message);
            };
            channel.BasicConsume(queue: createQueue,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine($"Monitoring queue: {createQueue}");

        }
    }
}
