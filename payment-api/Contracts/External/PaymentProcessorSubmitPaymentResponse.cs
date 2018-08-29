namespace payment_api.Contracts.External
{
    public class PaymentProcessorSubmitPaymentResponse
    {
        public string PaymentId { get; set; }

        public string PaymentStatus { get; set; }
    }
}