namespace payment_api.Contracts.External
{
    public class PaymentProcessorGetPaymentStatusResponse
    {
        public string PaymentId { get; set; }

        public decimal PaymentAmount { get; set; }

        public string PaymentStatus { get; set; }
    }
}