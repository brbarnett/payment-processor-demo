namespace payment_processor.Models
{
    public class Payment
    {
        public string Id { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }
    }
}