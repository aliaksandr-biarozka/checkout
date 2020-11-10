namespace Domain
{
    public class Money
    {
        public string Currency { get; }

        public long Value { get; }

        public Money(string currency, long amount)
        {
            Currency = currency;
            Value = amount;
        }
    }
}
