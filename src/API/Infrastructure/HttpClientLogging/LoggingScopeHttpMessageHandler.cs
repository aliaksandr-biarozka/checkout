using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.HttpClientLogging
{
    public class LoggingScopeHttpMessageHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public LoggingScopeHttpMessageHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var stopwatch = ValueStopwatch.StartNew();

            using (Log.BeginRequestPipelineScope(_logger, request))
            {
                Log.RequestPipelineStart(_logger, request);
                var response = await base.SendAsync(request, cancellationToken);
                Log.RequestPipelineEnd(_logger, response, stopwatch.GetElapsedTime());

                return response;
            }
        }

        private static class Log
        {
            private static class EventIds
            {
                public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
                public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");
            }

            private static readonly Func<ILogger, HttpMethod, Uri, string, IDisposable> _beginRequestPipelineScope =
                LoggerMessage.DefineScope<HttpMethod, Uri, string>(
                    "HTTP {HttpMethod} {Uri} {CorrelationId}");

            private static readonly Action<ILogger, HttpMethod, Uri, string, Exception> _requestPipelineStart =
                LoggerMessage.Define<HttpMethod, Uri, string>(
                    LogLevel.Information,
                    EventIds.PipelineStart,
                    "Start processing HTTP request {HttpMethod} {Uri} [Correlation: {CorrelationId}]");

            private static readonly Action<ILogger, double, HttpStatusCode, Exception> _requestPipelineEnd =
                LoggerMessage.Define<double, HttpStatusCode>(
                    LogLevel.Information,
                    EventIds.PipelineEnd,
                    "End processing HTTP request after {ElapsedMilliseconds}ms - {StatusCode}");

            public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request)
            {
                var correlationId = GetCorrelationIdFromRequest(request);

                return _beginRequestPipelineScope(logger, request.Method, request.RequestUri, correlationId);
            }

            public static void RequestPipelineStart(ILogger logger, HttpRequestMessage request)
            {
                var correlationId = GetCorrelationIdFromRequest(request);

                _requestPipelineStart(logger, request.Method, request.RequestUri, correlationId, null);
            }

            public static void RequestPipelineEnd(ILogger logger, HttpResponseMessage response, TimeSpan duration)
            {
                _requestPipelineEnd(logger, duration.TotalMilliseconds, response.StatusCode, null);
            }

            private static string GetCorrelationIdFromRequest(HttpRequestMessage request)
            {
                var correlationId = "Not set";

                if (request.Headers.TryGetValues("X-Correlation-ID", out var values))
                {
                    correlationId = values.First();
                }

                return correlationId;
            }
        }
    }
}
