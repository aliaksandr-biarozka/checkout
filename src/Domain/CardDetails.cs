using Domain.SeedWork;

namespace Domain
{
    public class CardDetails
    {
        private CardDetails() { }

        public string Number { get; private set; }

        public string Name { get; private set; }

        public string ExpiryMonth { get; private set; }

        public string ExpiryYear { get; private set; }

        public static CardDetails From(Card card)
        {
            Require.That(card != null, "Card is not provided");

            return new CardDetails
            {
                Number = GetMaskedNumber(card.Number),
                ExpiryMonth = card.ExpiryMonth,
                ExpiryYear = card.ExpiryYear,
                Name = card.Name
            };
        }

        private static string GetMaskedNumber(string number)
        {
            var digits = number.ToCharArray();

            var i = digits.Length - 4;
            while (--i > 0) digits[i] = '*';

            return new string(digits);
        }
    }
}
