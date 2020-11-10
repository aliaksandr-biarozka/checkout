using System;
using Domain.SeedWork;

namespace Domain
{
    public class Payment
    {
        private Guid _id;

        public Guid Id => _id;

        public Guid MerchantId { get; }

        public PaymentStatus Status { get; }

        public Guid AcquiringBankTransactionId { get; }

        public CardDetails CardDetails { get; }

        public Money Amount { get; }

        public Payment(CardDetails cardDetails, Money amound, PaymentStatus status, Guid acquiringBankTransactionId, Guid merchantId)
        {
            Require.That(cardDetails != null, "Card details are not provided");
            Require.That(amound != null, "Amount is not provided");

            CardDetails = cardDetails;
            Amount = amound;
            Status = status;
            AcquiringBankTransactionId = acquiringBankTransactionId;
            MerchantId = merchantId;
        }
    }    
}
