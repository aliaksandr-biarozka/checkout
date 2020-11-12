using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                await _next(context);
            }
            finally
            {
                _logger.LogInformation(
                    "Request {method} {url} => {statusCode} {newLine} {headers}",
                    context.Request?.Method,
                    context.Request?.GetDisplayUrl(),
                    context.Response?.StatusCode,
                    Environment.NewLine,
                    context.Request.Headers);
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder applicationBuilder)
            => applicationBuilder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
