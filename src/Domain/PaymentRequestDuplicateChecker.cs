using System.Threading.Tasks;

namespace Domain
{
    public interface IPaymentRequestDuplicateChecker
    {
        /// <summary>
        /// Check whether payment has already been processed.
        /// Exception is thrown if duplicate payment is detected
        /// </summary>
        /// <exception cref="SeedWork.DomainRuleViolationException" />
        /// <param name="requestId"></param>
        /// <param name="payment"></param>
        Task Check(PaymentRequest request);
    }
}
