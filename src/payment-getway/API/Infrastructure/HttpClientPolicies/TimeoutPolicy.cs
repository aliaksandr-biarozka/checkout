using System.Net.Http;
using Polly;

namespace API.Infrastructure.HttpClientPolicies
{
    public static class TimeoutPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> Basic(int requestTimeoutInSeconds) => Policy.TimeoutAsync<HttpResponseMessage>(requestTimeoutInSeconds);
    }
}
