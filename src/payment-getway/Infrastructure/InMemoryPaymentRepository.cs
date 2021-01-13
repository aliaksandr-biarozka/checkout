using System;
using System.Reflection;
using System.Threading.Tasks;
using Domain;
using StackExchange.Redis;

namespace Infrastructure
{
    // simulate repository. MemoryCache simulates db context object
    public class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly ConnectionMultiplexer _connection;

        private readonly Func<Guid, Guid, string> _id = (merchantId, paymentId) => $"{merchantId}_{paymentId}";

        public InMemoryPaymentRepository(ConnectionMultiplexer connection) => _connection = connection;

        public async Task Save(Payment payment)
        {
            // id should be set by db or id generator.
            // usually it is set by orm via reflection or it is set while object creation and provided by external id generator
            // code above "mimics" orm behavior
            var id = Guid.NewGuid();
            payment.SetProperty(nameof(Payment.Id), id);
            
            await _connection.GetDatabase().StringSetAsync(_id(payment.Id, payment.MerchantId), payment);
        }

        public async Task<Payment> Get(Guid paymentId, Guid merchantId)
        {
            var payment = await _connection.GetDatabase().StringGetAsync<Payment>(_id(paymentId, merchantId));

            if(payment == null)
            {

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

        public static void SetProperty(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public).SetValue(obj, value);
        }
    }
}
