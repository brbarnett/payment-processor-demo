namespace payment_processor.Contracts.External
{
    public class PaymentGatewaySubmitPaymentRequest
    {
        public string PaymentId { get; set; }
        
        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        public PaymentGatewaySubmitPaymentRequest(string paymentId, string accountNumber, decimal paymentAmount)
        {
            this.PaymentId = paymentId;
            this.AccountNumber = accountNumber;
            this.PaymentAmount = paymentAmount;
        }
    }
}