using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace API.Infrastructure.HttpClientPolicies
{
    public static class RetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> Basic(ILogger logger, HttpRequestMessage request, int retryAttempts) =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)),
                    async (outcome, timeSpan, retryAttempt, context) =>
                    {
                        logger.LogWarning($"Delaying for {timeSpan.TotalSeconds} sec, then making retry {retryAttempt}. Body: {await request.Content.ReadAsStringAsync()}");
                    });

        public static IAsyncPolicy<HttpResponseMessage> HonouringRetry(ILogger logger, HttpRequestMessage request, int retryAttempts)
        {
            return Policy<HttpResponseMessage>.HandleResult(r => r.StatusCode == (HttpStatusCode)429)
                .WaitAndRetryAsync(retryAttempts, (retryAttempt, response, context) =>
                {
                    var serverWaitDuration = GetServerWaitDuration(response);
                    var waitDuration = Math.Max(TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)).TotalMilliseconds,
                        serverWaitDuration.TotalMilliseconds);

                    return TimeSpan.FromMilliseconds(waitDuration);

                }, async (response, timeSpan, retryAttempt, context) =>
                {
                    logger.LogWarning($"Handle 429 http status code. Delaying for {timeSpan.TotalSeconds} sec, then making retry {retryAttempt}. Body: {await request.Content.ReadAsStringAsync()}");
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
