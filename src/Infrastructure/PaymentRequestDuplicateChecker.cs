using System;
using System.Threading.Tasks;
using Domain;
using Domain.SeedWork;
using LazyCache;

namespace Infrastructure
{
    public class PaymentRequestDuplicateChecker : IPaymentRequestDuplicateChecker
    {
        // in real app is should be distrebuted as there will be multiple nodes in the payment getway claster
        private readonly IAppCache _cache;

        private readonly Func<Guid, Guid, string> _key = (requestId, merchantId) => $"m_{merchantId}_r_{requestId}";

        public PaymentRequestDuplicateChecker(IAppCache cache) => _cache = cache;

        // logic that checks for duplicates should be added here.
        // simpulate duplication checking
        public async Task Check(PaymentRequest request)
        {
            var entry =_cache.GetOrAdd(_key(request.RequestId, request.MerchantId), () => request);

            if (entry != request)
                throw new DomainRuleViolationException($"Request {request.RequestId} has already been processed");

            await Task.CompletedTask;
        }
    }
}
