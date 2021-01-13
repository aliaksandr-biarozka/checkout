using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;

namespace API.Infrastructure.HttpClientPolicies
{
    public static class CircuitBreakerPolicy
    {     
        public static IAsyncPolicy<HttpResponseMessage> Basic(int allowedUnsuccessfulCalls, int durationOfBreakInSeconds) =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .CircuitBreakerAsync(allowedUnsuccessfulCalls, TimeSpan.FromSeconds(durationOfBreakInSeconds),
                (result, breakDuration) => throw new BrokenCircuitException("Service inoperative. Circuit is open"),
                () => { });
    }
}
