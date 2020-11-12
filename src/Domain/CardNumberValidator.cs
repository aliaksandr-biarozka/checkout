using System;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.SeedWork;

namespace Domain
{
    internal static class CardNumberValidator
    {
        private static readonly Func<char, int> CharToInt = c => c - '0';

        private static readonly Func<int, bool> IsEven = i => i % 2 == 0;

        private static readonly Func<int, int> DoubleDigit = i => (i * 2).ToString().ToCharArray().Select(CharToInt).Sum();

        internal static void Validate(string cardNumber)
        {
            Require.That(!String.IsNullOrWhiteSpace(cardNumber) && Regex.IsMatch(cardNumber, "^[0-9]{13,19}$", RegexOptions.Compiled), "The card number length must be 13 to 19 digits");

            var checkSum = cardNumber
                .ToCharArray()
                .Select(CharToInt)
                .Reverse()
                .Select((digit, index) => IsEven(index + 1) ? DoubleDigit(digit) : digit)
                .Sum();

            Require.That(checkSum % 10 == 0, $"The card number {cardNumber} is invalid");
        }
    }
}
