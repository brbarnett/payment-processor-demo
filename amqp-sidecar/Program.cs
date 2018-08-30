﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using amqp_sidecar.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace amqp_sidecar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureBrokerMessageHandler();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void ConfigureBrokerMessageHandler()
        {
            // mock config until I figure out dynamic configs
            BrokerConfig config = new BrokerConfig
            {
                Rules = new List<Rule>{
                    new Rule
                    {
                        Exchange = "payments",
                        Queue = "create",
                        RoutingKeys = new [] {"payments.create"},
                        EndpointUri = "http://payment-processor"
                        }
                }.AsEnumerable()
            };

            var httpClient = new HttpClient();
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            foreach (var rule in config.Rules)
            {
                channel.ExchangeDeclare(exchange: rule.Exchange, type: "topic");

                channel.QueueDeclare(queue: rule.Queue,
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false);

                foreach (var routingKey in rule.RoutingKeys)
                {
                    channel.QueueBind(queue: rule.Queue,
                                  exchange: rule.Exchange,
                                  routingKey: routingKey);
                }

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    httpClient.PostAsJsonAsync(rule.EndpointUri, message);
                };

                channel.BasicConsume(queue: rule.Queue,
                                 autoAck: true,
                                 consumer: consumer);

                Console.WriteLine($"Monitoring queue: {rule.Queue}");
            }
        }
    }
}
