namespace payment_api.Contracts
{
    public class SubmitPaymentResponse
    {
        public string AccountNumber { get; set; }

        public decimal PaymentAmount { get; set; }

        public SubmitPaymentResponse()
        {

        }
    }
}