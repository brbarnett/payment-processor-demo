using System;

namespace payment_processor.Models
{
    public class Payment
    {
        public string Id { get; set; }

        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }

        public Payment() { }

        public Payment(string accountNumber, decimal amount)
        {
            this.Id = Guid.NewGuid().ToString();
            this.AccountNumber = accountNumber;
            this.Amount = amount;
            this.Status = "Pending";
        }
    }
}