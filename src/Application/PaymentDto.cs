using System;
namespace Application
{
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public PaymentStatus Status { get; set; }

        public long Amount { get; set; }

        public string Currency { get; set; }
    }

    public enum PaymentStatus
    {
        Approved,
        Rejected
    }
}
