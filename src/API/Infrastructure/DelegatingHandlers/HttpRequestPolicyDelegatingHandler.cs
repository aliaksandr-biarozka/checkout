using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using API.Infrastructure.HttpClientPolicies;
using CircuitBreakerPolicy = API.Infrastructure.HttpClientPolicies.CircuitBreakerPolicy;
using Microsoft.Extensions.DependencyInjection;

namespace API.Infrastructure.DelegatingHandlers
{
    public class HttpRequestPolicyDelegatingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (BrokenCircuitException e)
            {
                throw new BrokenCircuitException($"Circuit breaker is open after {CircuitBreakerPolicy.AllowedUnsuccessfulCalls} consecutive unsuccessful attempts to access third-party API: {request.Method.Method}: {request.RequestUri}", e);
            }
            catch (TaskCanceledException e)
            {
                if (e.ToString().Contains("Polly.Retry"))
                {
                    var body = String.Empty;
                    if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                    {
                        body = await request.Content.ReadAsStringAsync();
                    }

                    throw new HttpRequestException($"{RetryPolicy.RetryAttempts} unsuccessful attempts have been made to reach third-party API: {request.Method.Method}: {request.RequestUri}, body: {body}", e);
                }

                throw;
            }
        }
    }

    public static class HttpClientBuilderPolicyExtensions
    {
        public static IHttpClientBuilder AddHttpRequestPolicyHandlerSupport(this IHttpClientBuilder httpClientBuilder)
        {
            return httpClientBuilder.AddHttpMessageHandler<HttpRequestPolicyDelegatingHandler>();
        }
    }

    public static class ServicesCollectionHttpPolicyDelegatingExtensions
    {
        public static IServiceCollection AddHttRequestPolicyDelegating(this IServiceCollection services)
        {
            return services.AddTransient<HttpRequestPolicyDelegatingHandler>();
        }
    }
}
