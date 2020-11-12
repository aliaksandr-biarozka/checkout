using System;
using System.Text.RegularExpressions;
using Domain.SeedWork;

namespace Domain
{
    public class Card
    {
        public Card(string number, string name, string expiryMonth, string expiryYear, string cvv)
        {
            Require.That(!String.IsNullOrWhiteSpace(name) && name.Length >= 2 && name.Length <= 26, $"The card holder name must be 2 to 26 characters");
            Require.That(!String.IsNullOrWhiteSpace(cvv) && Regex.IsMatch(cvv, @"^\d{3,4}$", RegexOptions.Compiled), "The cvv/cvc must be 3 to 4 digits");
            Require.That(!String.IsNullOrWhiteSpace(expiryMonth) && Regex.IsMatch(expiryMonth, "0[1-9]|1[0-2]", RegexOptions.Compiled), $"The expiry month must be 2 digits");
            Require.That(!String.IsNullOrWhiteSpace(expiryYear) && Regex.IsMatch(expiryYear, @"[2-9]\d{3}", RegexOptions.Compiled), $"The expiry year must be 4 digits");

            var year = Int32.Parse(expiryYear);
            Require.That(year > DateTime.UtcNow.Year || year == DateTime.UtcNow.Year && Int32.Parse(expiryMonth) >= DateTime.UtcNow.Month, "Provided card is expired");

            CardNumberValidator.Validate(number);

            Number = number;
            Name = name;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Cvv = cvv;
        }

        public string Number { get; }

        public string Name { get; }

        public string ExpiryMonth { get; }

        public string ExpiryYear { get; }

        public string Cvv { get; }
    }
}
