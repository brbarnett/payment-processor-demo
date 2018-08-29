namespace payment_processor.Contracts
{
    public class SubmitPaymentResponse
    {
        public string PaymentId { get; set; }

        public string PaymentStatus { get; set; }

        public SubmitPaymentResponse(string paymentId, string paymentStatus)
        {
            this.PaymentId = paymentId;
            this.PaymentStatus = paymentStatus;
        }
    }
}