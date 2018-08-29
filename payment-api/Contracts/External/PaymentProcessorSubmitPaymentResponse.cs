namespace payment_api.Contracts
{
    public class PaymentProcessorSubmitPaymentResponse
    {
        public string PaymentId { get; set; }

        public string PaymentStatus { get; set; }
    }
}