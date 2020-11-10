using System;
using System.Threading.Tasks;

namespace Domain
{
    public interface IPaymentRepository
    {
        Task Save(Payment payment);

        Task<Payment> Get(Guid paymentId, Guid merchantId);
    }
}
