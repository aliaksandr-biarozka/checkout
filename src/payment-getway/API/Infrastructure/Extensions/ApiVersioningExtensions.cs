using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace API.Infrastructure.Extensions
{
    internal static class ApiVersioningExtensions
    {
        public static IServiceCollection AddConfiguredApiVersioning(this IServiceCollection services)
        {
            return services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
        }
    }
}