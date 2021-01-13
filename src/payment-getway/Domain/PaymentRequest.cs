using System;
using Domain.SeedWork;

namespace Domain
{
    public class PaymentRequest
    {
        public PaymentRequest(Guid merchantId, Guid requestId, Card card, Money amount)
        {
            Require.That(card != null, "Card is not provided");
            Require.That(amount != null, "Amount is not provided");

            MerchantId = merchantId;
            RequestId = requestId;
            Card = card;
            Amount = amount;
        }

        public Guid MerchantId { get; }

        public Guid RequestId { get; }

        public Card Card { get; }

        public Money Amount { get; }
    }
}
