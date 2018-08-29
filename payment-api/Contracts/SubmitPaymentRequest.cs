namespace payment_api.Contracts
{
    public class SubmitPaymentRequest
    {
        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }
    }
}