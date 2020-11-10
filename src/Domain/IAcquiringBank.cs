using System;
using System.Threading.Tasks;

namespace Domain
{
    public interface IAcquiringBank
    {
        Task<PaymentResult> Process(PaymentRequest paymentRequest);
    }

    public class PaymentResult
    {
        public PaymentResult(Guid acquiringBankTransactionId, PaymentStatus status)
        {
            AcquiringBankTransactionId = acquiringBankTransactionId;
            Status = status;
        }

        public Guid AcquiringBankTransactionId { get; }

        public PaymentStatus Status { get; }
    }
}
