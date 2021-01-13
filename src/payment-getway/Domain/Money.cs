using Domain.SeedWork;

namespace Domain
{
    public class Money
    {
        private const long MaxValue = 999_999_999_999_999_999;

        public string Currency { get; }

        public long Value { get; }

        public Money(string currency, long value)
        {
            CurrencyValidator.Validate(currency);
            Require.That(value > 0 && value <= MaxValue, $"Provided amount must be between 1 and {MaxValue}");

            Currency = currency;
            Value = value;
        }
    }
}
