using System;
using System.Reflection;
using System.Threading.Tasks;
using Domain;
using LazyCache;

namespace Infrastructure
{
    // simulate repository. MemoryCache simulates db context object
    public class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly IAppCache _cache;

        public InMemoryPaymentRepository(IAppCache cache) => _cache = cache;

        public async Task Save(Payment payment)
        {
            // id should be set by db or id generator.
            // usually it is set by orm via reflection or it is set while object creation and provided by external id generator
            // code above "mimics" orm behavior
            var id = Guid.NewGuid();
            payment.SetField("_id", id);

            _cache.Add(id.ToString(), payment);

            await Task.CompletedTask;
        }

        public async Task<Payment> Get(Guid paymentId, Guid merchantId)
        {
            await Task.CompletedTask;

            var payment = _cache.Get<Payment>(paymentId.ToString());

            if(payment != null && payment.MerchantId != merchantId)
            {
                return null;
            }

            return payment;
        }

        
    }

    internal static class PrivateInjector
    {
        private static FieldInfo GetPrivateField(Type type, string propertyName)
        {
            return type.GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void SetField(this object obj, string fieldName, object value)
        {
            GetPrivateField(obj.GetType(), fieldName).SetValue(obj, value);
        }
    }
}
