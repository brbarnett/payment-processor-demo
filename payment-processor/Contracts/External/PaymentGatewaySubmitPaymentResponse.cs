namespace payment_processor.Contracts.External
{
    public class PaymentGatewaySubmitPaymentResponse
    {
        public string ExternalPaymentId {get;set;}

        public string PaymentStatus {get;set;}
    }
}