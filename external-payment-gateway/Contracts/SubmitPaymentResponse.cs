namespace external_payment_gateway.Contracts
{
    public class SubmitPaymentResponse
    {
        public string ExternalPaymentId {get;set;}

        public string PaymentStatus {get;set;}

        public SubmitPaymentResponse(string externalPaymentId, string paymentStatus)
        {
            this.ExternalPaymentId = externalPaymentId;
            this.PaymentStatus = paymentStatus;
        }
    }
}