using System;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure.HttpClientLogging
{
    public class LoggingFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILoggerFactory _loggerFactory;

        public LoggingFilter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return (builder) =>
            {
                // Run other configuration first, we want to decorate.
                next(builder);

                var loggerName = !string.IsNullOrEmpty(builder.Name) ? builder.Name : "Default";

                var outerLogger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.LogicalHandler");

                builder.AdditionalHandlers.Insert(0, new LoggingScopeHttpMessageHandler(outerLogger));
            };
        }
    }
}
