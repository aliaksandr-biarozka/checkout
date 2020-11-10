using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace API.Infrastructure.HttpClientPolicies
{
    // numbers are hardcoded for implicity. They can be read from configuration
    public static class RetryPolicy
    {
        public const int RetryAttempts = 5;

        public static IAsyncPolicy<HttpResponseMessage> Basic(IServiceProvider serviceProvider, HttpRequestMessage request) =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(RetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)),
                    async (outcome, timeSpan, retryAttempt, context) =>
                    {
                        serviceProvider.GetService<ILogger<HttpClient>>()
                            .LogInformation($"Delaying for {timeSpan.TotalSeconds} sec, then making retry {retryAttempt} calling {request.Method.Method}: {request.RequestUri}. Body: {await request.Content.ReadAsStringAsync()}");
                    });

        public static IAsyncPolicy<HttpResponseMessage> HonouringRetry(IServiceProvider serviceProvider, HttpRequestMessage request)
        {
            return Policy<HttpResponseMessage>.HandleResult(r => r.StatusCode == (HttpStatusCode)429)
                .WaitAndRetryAsync(RetryAttempts, (retryAttempt, response, context) =>
                {
                    var serverWaitDuration = GetServerWaitDuration(response);
                    var waitDuration = Math.Max(TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)).TotalMilliseconds,
                        serverWaitDuration.TotalMilliseconds);

                    return TimeSpan.FromMilliseconds(waitDuration);

                }, async (response, timeSpan, retryAttempt, context) =>
                {
                    serviceProvider.GetService<ILogger<HttpClient>>().LogInformation($"Handle 429 http status code. Delaying for {timeSpan.TotalSeconds} sec, then making retry {retryAttempt} calling {request.Method.Method}: {request.RequestUri}. Body: {await request.Content.ReadAsStringAsync()}");
                });
        }

        private static TimeSpan GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
        {
            var retryAfter = response?.Result?.Headers?.RetryAfter;
            if (retryAfter == null)
                return TimeSpan.Zero;

            return retryAfter.Date.HasValue
                ? retryAfter.Date.Value - DateTime.UtcNow
                : retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
        }
    }
}
