using System.Threading.Tasks;
using Domain;
using Domain.SeedWork;
using StackExchange.Redis;

namespace Infrastructure
{
    public class PaymentRequestDuplicateChecker : IPaymentRequestDuplicateChecker
    {
        private readonly ConnectionMultiplexer _connection;

        public PaymentRequestDuplicateChecker(ConnectionMultiplexer connection) => _connection = connection;

        public async Task Check(PaymentRequest request)
        {
            var saved = await _connection.GetDatabase().StringSetAsync($"m_{request.MerchantId}_r_{request.RequestId}", request, when: When.NotExists);

            if (!saved)
                throw new DomainRuleViolationException($"Request {request.RequestId} made by merchant {request.MerchantId} has already been processed");
        }
    }
}
