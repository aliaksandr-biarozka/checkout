using System;
using Domain.SeedWork;

namespace Domain
{
    public class Card
    {
        public Card(string number, string name, string expiryMonth, string expiryYear, string cvv)
        {
            Require.That(!String.IsNullOrWhiteSpace(number), $"{nameof(number)} is not provided");
            Require.That(!String.IsNullOrWhiteSpace(name), $"{nameof(name)} is not provided");
            Require.That(!String.IsNullOrWhiteSpace(expiryMonth), $"{nameof(expiryMonth)} is not provided");
            Require.That(!String.IsNullOrWhiteSpace(expiryYear), $"{nameof(expiryYear)} is not provided");
            Require.That(!String.IsNullOrWhiteSpace(cvv), $"{nameof(cvv)} is not provided");

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
