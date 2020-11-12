using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Infrastructure
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

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
            catch(Exception e)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                ProblemDetails errorData;
                if(e is ApplicationServiceException)
                {
                    errorData = new ProblemDetails { Detail = e.Message, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3", Status = (int)HttpStatusCode.Forbidden } ;

                    context.Response.StatusCode = errorData.Status.Value;
                }
                else
                {
                    errorData = new ProblemDetails { Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1", Status = (int)HttpStatusCode.InternalServerError };

                    context.Response.StatusCode = errorData.Status.Value;

#if DEBUG
                    errorData.Detail = e.Message;
#else
                    errorData.Detail = "Error was occurred. Please try again later!";
#endif

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

                    _logger.LogError(e, errorData.Detail, method, url, user, host, body);
                }

                await context.Response.WriteAsync(JsonSerializer.Serialize<ProblemDetails>(errorData));
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
