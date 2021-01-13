using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace API.Infrastructure.Logging
{
    internal static class SerilogExtensions
    {
        internal static LoggerConfiguration InitializeFromAppSettings(this LoggerConfiguration loggerConfiguration)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return loggerConfiguration.ReadFrom.Configuration(configuration);
        }
    }
}
