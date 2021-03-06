﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using payment_processor.Contracts;
using payment_processor.Contracts.External;
using payment_processor.Models;
using StackExchange.Redis;

namespace payment_processor
{
    [Route("/")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IConnectionMultiplexer _cacheConnection;
        private readonly HttpClient _httpClient;

        private static string ExternalPaymentGatewayUri = Environment.GetEnvironmentVariable("EXTERNAL_PAYMENT_GATEWAY_URI");

        public Controller(IConnectionMultiplexer cacheConnection, HttpClient HttpClient)
        {
            this._cacheConnection = cacheConnection;
            this._httpClient = HttpClient;
        }

        [HttpGet("{paymentId}")]
        public async Task<ActionResult<GetPaymentStatusResponse>> GetPaymentStatus(string paymentId)
        {
            Console.WriteLine($"Received status request for paymentId: {paymentId}");
            
            // get payment from cache
            var cache = this._cacheConnection.GetDatabase();
            RedisValue cachedValue = await cache.StringGetAsync($"payment.{paymentId}");

            if (cachedValue.IsNullOrEmpty) return new GetPaymentStatusResponse(paymentId, "Not found");

            var payment = JsonConvert.DeserializeObject<Payment>(cachedValue);

            return new GetPaymentStatusResponse(payment.Id, payment.Status);
        }

        [HttpPost("")]
        public async Task<ActionResult<SubmitPaymentResponse>> CreatePaymentSync([FromBody] SubmitPaymentRequest paymentRequest)
        {
            Console.WriteLine($"Recieved message: {paymentRequest}");

            Payment payment = new Payment(paymentRequest.PaymentId, paymentRequest.AccountNumber, paymentRequest.PaymentAmount);

            // save payment to cache
            var cache = this._cacheConnection.GetDatabase();
            await cache.StringSetAsync($"payment.{payment.Id}", JsonConvert.SerializeObject(payment));

            // synchronously wait for external payment gateway
            PaymentGatewaySubmitPaymentRequest gatewayRequest = new PaymentGatewaySubmitPaymentRequest(payment.Id, payment.AccountNumber, payment.Amount);
            var serviceResponse = await this._httpClient.PostAsJsonAsync(ExternalPaymentGatewayUri, gatewayRequest);
            if (!serviceResponse.IsSuccessStatusCode) throw new Exception("Error processing payment");

            PaymentGatewaySubmitPaymentResponse gatewayResponse = await serviceResponse.Content.ReadAsAsync<PaymentGatewaySubmitPaymentResponse>(); ;

            // update payment in cache
            payment.Status = gatewayResponse.PaymentStatus;
            await cache.StringSetAsync($"payment.{payment.Id}", JsonConvert.SerializeObject(payment));
            
            return new SubmitPaymentResponse(payment.Id, payment.Status);
        }
    }
}
