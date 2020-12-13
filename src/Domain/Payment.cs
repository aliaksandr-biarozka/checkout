using System;
using Domain.SeedWork;

namespace Domain
{
    public class Payment
    {
        public Guid Id { get; private set; }

        public Guid MerchantId { get; }

        public PaymentStatus Status { get; }

        public Guid AcquiringBankTransactionId { get; }

        public CardDetails CardDetails { get; }

        public Money Amount { get; }

        public Payment(CardDetails cardDetails, Money amount, PaymentStatus status, Guid acquiringBankTransactionId, Guid merchantId)
        {
            Require.That(cardDetails != null, "Card details are not provided");
            Require.That(amount != null, "Amount is not provided");

            CardDetails = cardDetails;
            Amount = amount;
            Status = status;
            AcquiringBankTransactionId = acquiringBankTransactionId;
            MerchantId = merchantId;
        }
    }    
}
