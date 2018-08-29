using System;

namespace payment_api.Contracts.External
{
    public class PaymentProcessorSubmitPaymentRequest
    {
        public string PaymentId {get;set;}

        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        public PaymentProcessorSubmitPaymentRequest(string accountNumber, decimal paymentAmount)
        {
            // auto-generate ID on request
            this.PaymentId = Guid.NewGuid().ToString();
            this.AccountNumber = accountNumber;
            this.PaymentAmount = paymentAmount;
        }
    }
}