namespace Application
{
    public record PaymentReqestDto
    {
        public string CardNumber { get; init; }

        public string CardHolderName { get; init; }

        public string ExpiryMonth { get; init; }

        public string ExpiryYear { get; init; }

        public string CVV { get; init; }

        public long Amount { get; init; }

        public string Currency { get; init; }
    }
}
