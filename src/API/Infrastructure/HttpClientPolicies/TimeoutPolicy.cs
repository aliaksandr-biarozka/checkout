using System.Net.Http;
using Polly;

namespace API.Infrastructure.HttpClientPolicies
{
    public static class TimeoutPolicy
    {
        // number is hardcoded for implicity. it can be read from configuration
        public static IAsyncPolicy<HttpResponseMessage> Basic => Policy.TimeoutAsync<HttpResponseMessage>(30);
    }
}
