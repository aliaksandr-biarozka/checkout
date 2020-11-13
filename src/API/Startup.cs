using System;
using System.IO;
using API.Infrastructure;
using API.Infrastructure.Extensions;
using API.Infrastructure.HttpClientLogging;
using API.Infrastructure.HttpClientPolicies;
using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly;
using Swashbuckle.AspNetCore.Filters;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                // System.Text.Json does not support snake case strategy. Once it does (.NET 6), System.Text.Json should be used for better performance 
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { AllowIntegerValues = false });
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };

                    JsonConvert.DefaultSettings = () => options.SerializerSettings;
                });

            services.AddConfiguredApiVersioning();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Payment Getway API",
                    Description = "Payment Getway API",
                    Version = "1.0"
                });

                options.ExampleFilters();

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Documentation.xml"));
            }).AddSwaggerGenNewtonsoftSupport()
            .AddSwaggerExamplesFromAssemblyOf<Startup>();

            services.AddLazyCache();
            services.AddTransient<PaymentService>();
            services.AddTransient<IPaymentRepository, InMemoryPaymentRepository>();
            services.AddTransient<IAcquiringBankConfigurationProvider>(p => new AcquiringBankConfigurationProvider(Configuration.GetSection(nameof(AcquiringBankConfiguration)).Get<AcquiringBankConfiguration>()));
            services.AddTransient<IPaymentRequestDuplicateChecker, PaymentRequestDuplicateChecker>();

            var httpPolicySettings = Configuration.GetSection(nameof(HttpClientPolicySettings)).Get<HttpClientPolicySettings>();
            var circuitBreaker = CircuitBreakerPolicy.Basic(httpPolicySettings.RetryCountBeforeBreaking, httpPolicySettings.DurationOfBreakInSeconds);            
            services.AddHttpClient<IAcquiringBank, AcquiringBank>()
                .AddPolicyHandler((s, r) => Policy.WrapAsync(RetryPolicy.Basic(s.GetService<ILogger<IAcquiringBank>>(), r, httpPolicySettings.RetryCount),
                                                             RetryPolicy.HonouringRetry(s.GetService<ILogger<IAcquiringBank>>(), r, httpPolicySettings.RetryCount)))
                .AddPolicyHandler(TimeoutPolicy.Basic(httpPolicySettings.RequestTimeoutInSeconds))
                .AddPolicyHandler((s, r) => circuitBreaker);

            services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, LoggingFilter>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCustomExceptionHandling()
               .UseRequestLogging();

            app.UseRouting();

            app.UseSwagger(options => options.RouteTemplate = "{documentName}/swagger.json");

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/v1/swagger.json", $"Payment Getway API");

                c.RoutePrefix = "";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
