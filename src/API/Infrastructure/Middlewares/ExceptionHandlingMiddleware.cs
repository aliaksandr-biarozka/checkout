using System;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace API.Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

        private readonly string errorMessage = $"An error occured during processing request: {{method}} {{url}} made by user {{user}}{Environment.NewLine}Host: {{hostname}}{Environment.NewLine} Request body: {{body}}";

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var errorData = new ProblemDetails { Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1", Status = StatusCodes.Status500InternalServerError };

                context.Response.StatusCode = errorData.Status.Value;

#if DEBUG
                errorData.Detail = e.Message;
#else
                errorData.Detail = "Error was occurred. Please try again later!";
#endif
                if (e is BrokenCircuitException)
                {
                    _logger.LogWarning(e.Message);

                    return;
                }

                string user = null;
                string url = null;
                string method = null;
                string host = null;
                string body = null;

                try
                {
                    url = context.Request.GetDisplayUrl();
                    method = context.Request.Method;
                    host = context.Request.Host.Host;

                    if (MediaTypeNames.Application.Json == context.Request.ContentType)
                    {
                        if (context.Request.Body.CanSeek)
                        {
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                        }

                        using (var reader = new StreamReader(context.Request.Body))
                        {
                            body = await reader.ReadToEndAsync();
                        }
                    }
                }
                catch
                {
                    // ignored
                }

                _logger.LogError(e, errorMessage, method, url, user, host, body);

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorData));
            }
        }
    }

    public static class ExceptionMiddlewareExctensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}