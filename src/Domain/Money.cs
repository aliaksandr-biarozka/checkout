using System;
using System.Collections.Generic;
using Domain.SeedWork;

namespace Domain
{
    public class Money : ValueObject
    {
        public string Currency { get; }

        public long Value { get; }

        public Money(string currency, long amount)
        {
            Currency = currency;
            Value = amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Currency;
            yield return Value;
        }
    }
}
