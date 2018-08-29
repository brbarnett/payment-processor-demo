namespace payment_api.Contracts.External
{
    public class PaymentProcessorSubmitPaymentRequest
    {
        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        public PaymentProcessorSubmitPaymentRequest(string accountNumber, decimal paymentAmount)
        {
            this.AccountNumber = accountNumber;
            this.PaymentAmount = paymentAmount;
        }
    }
}