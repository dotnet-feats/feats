using System;
using System.Text.Json.Serialization;
using Feats.Evaluations.Features;
using Feats.Evaluations.Strategies;
using Feats.EventStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Prometheus;

namespace Feats.Evaluations
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IConfiguration>(this._configuration);
            services.TryAddSingleton<ISystemClock>(new SystemClock());
            services.AddLogging();
            
            services.AddResponseCaching();
            services.AddHttpContextAccessor();
            services
                .AddEventStore()
                .AddFeatures()
                .AddStrategies();
            services.AddRouting();
            services.AddMvcCore();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services
                .AddHealthChecks()
                .ForwardToPrometheus();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseResponseCaching();
            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
