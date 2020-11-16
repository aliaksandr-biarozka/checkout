using System;
using App.Metrics;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Infrastructure.Extensions
{
    internal static class ServiceCollectionExtensions
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

        public static IServiceCollection AddConfiguredAppMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var appMetrics = configuration.GetSection(nameof(ApplicationMetrics)).Get<ApplicationMetrics>();
            services.AddMetrics(AppMetrics.CreateDefaultBuilder().Report.ToInfluxDb(options =>
            {
                options.InfluxDb.BaseUri = new Uri(appMetrics.DataStore.Address);
                options.InfluxDb.Database = appMetrics.DataStore.Database;
                options.InfluxDb.UserName = appMetrics.DataStore.UserName;
                options.InfluxDb.Password = appMetrics.DataStore.Password;
                options.InfluxDb.Consistenency = appMetrics.DataStore.Consistenency;
                options.InfluxDb.RetentionPolicy = appMetrics.DataStore.RetentionPolicy;
                options.InfluxDb.CreateDataBaseIfNotExists = appMetrics.DataStore.CreateDataBaseIfNotExists;
                options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(appMetrics.HttpPolicy.BackoffPeriodInSeconds);
                options.HttpPolicy.FailuresBeforeBackoff = appMetrics.HttpPolicy.FailuresBeforeBackoff;
                options.HttpPolicy.Timeout = TimeSpan.FromSeconds(appMetrics.HttpPolicy.TimeoutInSeconds);
                options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                options.FlushInterval = TimeSpan.FromSeconds(appMetrics.FlushIntervalInSeconds);
            }).Build())
                .AddMetricsReportingHostedService()
                .AddMetricsTrackingMiddleware(options =>
                {
                    options.ApdexTSeconds = appMetrics.Tracking.ApdexTSeconds;
                    options.IgnoredHttpStatusCodes = appMetrics.Tracking.IgnoredHttpStatusCodes;
                });

            return services;
        }
    }
}