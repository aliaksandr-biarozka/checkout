using System.Collections.Generic;
using Domain.SeedWork;

namespace Domain
{
    internal static class CurrencyValidator
    {
        private static HashSet<string> _supportedCurrencies = new HashSet<string>(new[] { "EUR", "USD", "GBP", "JPY", "AUD", "CAD" });

        internal static void Validate(string currency)
        {
            Require.That(_supportedCurrencies.Contains(currency), "Provided currency is not supported");
        }
    }
}
