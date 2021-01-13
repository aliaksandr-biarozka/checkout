using API.Infrastructure.ActionResults;
using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace API.Infrastructure.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private ILogger _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger) => _logger = logger;

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApplicationServiceException)
            {
                var errorData = new ValidationProblemDetails
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = "Please refer to the errors property for additional details."
                };

                errorData.Errors.Add("domain_rule_violation", new string[] { context.Exception.Message });

                context.Result = new UnprocessableEntityObjectResult(errorData);
                context.HttpContext.Response.StatusCode = errorData.Status.Value;

                context.ExceptionHandled = true;
            }
            else if(context.Exception is BrokenCircuitException)
            {
                var errorData = new ProblemDetails { Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1", Status = StatusCodes.Status500InternalServerError };
#if DEBUG
                errorData.Detail = context.Exception.Message;
#else
                errorData.Detail = "Error was occurred. Please try again later!";
#endif

                _logger.LogWarning(context.Exception.Message);

                context.Result = new InternalServerErrorObjectResult(errorData);
                context.HttpContext.Response.StatusCode = errorData.Status.Value;

                context.ExceptionHandled = true;
            }
        }
    }
}
