using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;

namespace API.Infrastructure.HttpClientPolicies
{
    // number are hardcoded for implicity. They can be read from configuration
    public static class CircuitBreakerPolicy
    {     
        public const int AllowedUnsuccessfulCalls = 5;

        public static IAsyncPolicy<HttpResponseMessage> Basic =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(AllowedUnsuccessfulCalls, TimeSpan.FromSeconds(15),
                (result, breakDuration) => throw new BrokenCircuitException("Service inoperative. Circuit is open"),
                () => { });
    }
}
