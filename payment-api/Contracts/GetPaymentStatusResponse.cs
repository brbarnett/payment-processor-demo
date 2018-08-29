namespace payment_api.Contracts
{
    public class GetPaymentStatusResponse
    {
        public string PaymentId { get; set; }

        public decimal PaymentAmount { get; set; }

        public string PaymentStatus { get; set; }

        public GetPaymentStatusResponse(string paymentId, decimal paymentAmount, string paymentStatus)
        {
            this.PaymentId = paymentId;
            this.PaymentAmount = paymentAmount;
            this.PaymentStatus = paymentStatus;
        }
    }
}