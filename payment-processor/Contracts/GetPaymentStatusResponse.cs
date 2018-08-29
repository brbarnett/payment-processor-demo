namespace payment_processor.Contracts
{
    public class GetPaymentStatusResponse
    {
        public string PaymentId { get; set; }

        public string PaymentStatus { get; set; }

        public GetPaymentStatusResponse(string paymentId, string paymentStatus)
        {
            this.PaymentId = paymentId;
            this.PaymentStatus = paymentStatus;
        }
    }
}