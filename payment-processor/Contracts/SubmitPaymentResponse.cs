namespace payment_processor.Contracts
{
    public class SubmitPaymentResponse
    {
        public string PaymentId { get; set; }

        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        public string PaymentStatus { get; set; }

        public SubmitPaymentResponse(string paymentId, string accountNumber, decimal paymentAmount, string paymentStatus)
        {
            this.PaymentId = paymentId;
            this.AccountNumber = accountNumber;
            this.PaymentAmount = paymentAmount;
            this.PaymentStatus = paymentStatus;
        }
    }
}