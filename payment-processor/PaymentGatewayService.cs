using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using payment_processor.Contracts;
using payment_processor.Contracts.External;
using payment_processor.Models;
using StackExchange.Redis;

namespace payment_processor
{
    public interface IPaymentGatewayService
    {
        Task<Payment> Process(SubmitPaymentRequest paymentRequest);
    }

    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConnectionMultiplexer _cacheConnection;
        private readonly HttpClient _httpClient;

        public PaymentGatewayService(IConnectionMultiplexer cacheConnection, HttpClient httpClient)
        {
            this._cacheConnection = cacheConnection;
            this._httpClient = httpClient;
        }

        public async Task<Payment> Process(SubmitPaymentRequest paymentRequest)
        {
            Payment payment = new Payment(paymentRequest.PaymentId, paymentRequest.AccountNumber, paymentRequest.PaymentAmount);

            // save payment to cache
            var cache = this._cacheConnection.GetDatabase();
            await cache.StringSetAsync($"payment.{payment.Id}", JsonConvert.SerializeObject(payment));

            // synchronously wait for external payment gateway
            PaymentGatewaySubmitPaymentRequest gatewayRequest = new PaymentGatewaySubmitPaymentRequest(payment.Id, payment.AccountNumber, payment.Amount);
            var serviceResponse = await this._httpClient.PostAsJsonAsync($"http://external-payment-gateway", gatewayRequest);
            if (!serviceResponse.IsSuccessStatusCode) throw new Exception("Error processing payment");

            PaymentGatewaySubmitPaymentResponse gatewayResponse = await serviceResponse.Content.ReadAsAsync<PaymentGatewaySubmitPaymentResponse>(); ;

            // update payment in cache
            payment.Status = gatewayResponse.PaymentStatus;
            await cache.StringSetAsync($"payment.{payment.Id}", JsonConvert.SerializeObject(payment));

            return payment;
        }
    }
}