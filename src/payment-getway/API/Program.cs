using System;
using System.Threading.Tasks;
using API.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace API
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .InitializeFromAppSettings()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                await CreateHostBuilder(args).Build().RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.Configure(options =>
                {
                    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId |
                                                      ActivityTrackingOptions.SpanId |
                                                      ActivityTrackingOptions.ParentId;
                });
            })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseKestrel(options => options.AddServerHeader = false)
                                                                  .UseStartup<Startup>());
    }
}
