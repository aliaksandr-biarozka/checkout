namespace Application
{
    public class PaymentReqestDto
    {
        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public string CVV { get; set; }

        public long Amount { get; set; }

        public string Currency { get; set; }
    }
}
