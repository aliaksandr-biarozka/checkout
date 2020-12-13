using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace API.Infrastructure.Extensions
{
    internal static class SerilogExtensions
    {
        internal static LoggerConfiguration InitializeFromAppSettings(this LoggerConfiguration loggerConfiguration)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return loggerConfiguration.ReadFrom.Configuration(configuration);
        }
    }
}
