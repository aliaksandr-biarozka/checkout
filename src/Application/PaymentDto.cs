using System;
namespace Application
{
    public record PaymentDto
    {
        public Guid PaymentId { get; init; }

        public string CardNumber { get; init; }

        public string CardHolderName { get; init; }

        public string ExpiryMonth { get; init; }

        public string ExpiryYear { get; init; }

        public PaymentStatus Status { get; init; }

        public long Amount { get; init; }

        public string Currency { get; init; }
    }

    public enum PaymentStatus
    {
        Approved,
        Rejected
    }
}
