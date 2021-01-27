using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MVCCoreDocker.Data;
using MVCCoreDocker.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MVCCoreDocker
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
            /*
            https://appdevmusings.com/configure-kubernetes-liveness-and-readiness-probes-for-asp-net-core-2-2-web-application-using-health-checks
            */
            services.AddSingleton<RandomHealthCheck>();

            services.AddSingleton<ReadinessHealthCheck>();
            services.AddSingleton<LivenessHealthCheck>();

            services.AddHealthChecks()
            .AddLivenessHealthCheck("Liveness", HealthStatus.Unhealthy, new List<string>() { "Liveness" })
            .AddReadinessHealthCheck("Readiness", HealthStatus.Unhealthy, new List<string> { "Readiness" });



            /*
            services
                .AddHealthChecks()
#if DEBUG
                .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //.AddCheck<RandomHealthCheck>("random");
#else
                .AddSqlServer(Configuration.GetConnectionString("DefaultConnectionRelease")); 
                //.AddCheck<RandomHealthCheck>("random");
#endif
            //.AddRabbitMQ(rabbitConnectionString: "amqp://localhost:5672", name: "rabbit1")
            //.AddRabbitMQ(rabbitConnectionString: "amqp://localhost:6672", name: "rabbit2")
            //.AddSqlServer(connectionString: Configuration["Data:ConnectionStrings:Sample"])


            //.AddIdentityServer(new Uri("http://localhost:6060"))
            //.AddAzureServiceBusQueue("Endpoint=sb://unaidemo.servicebus.windows.net/;SharedAccessKeyName=policy;SharedAccessKey=5RdimhjY8yfmnjr5L9u5Cf0pCFkbIM7u0HruJuhjlu8=", "que1")
            //.AddAzureServiceBusTopic("Endpoint=sb://unaidemo.servicebus.windows.net/;SharedAccessKeyName=policy;SharedAccessKey=AQhdhXwnkzDO4Os0abQV7f/kB6esTfz2eFERMYKMsKk=", "to1")
            //.AddApplicationInsightsPublisher(saveDetailedReport: true);
            */

            /*
             * https://medium.com/swlh/implement-health-checks-for-kubernetes-in-your-application-951eb483a05c
             AddCheck<MongoDbHealthCheck>(
                    "Dependency_Injection_health_check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" })
                .AddCheck(
                    "Direct_health_check",
                    new MongoDbHealthCheck("mongodb://localhost:27017/"),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" });
            */

#if DEBUG
            services.AddDbContext<SchoolContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
#else
             services.AddDbContext<SchoolContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionRelease")));
#endif

            services.AddDbContext<LapContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("LapConnection")));



            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllersWithViews();
        }

 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseHealthChecks("/health/live", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "Liveness"
            })
            .UseHealthChecks("/health/ready", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "Readiness",
            });


            /*
            app
                .UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true
                })
                .UseHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    //  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                //.UseHealthChecksPrometheusExporter("/metrics");
             
              https://dzone.com/articles/health-checks-with-aspnet-core-and-kubernetes
                        var healthCheckOptions = new HealthCheckOptions()
                        {
                          ///  ResponseWriter = WriteReadinessResponse
                        }
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHealthChecks("/ops/health", healthCheckOptions);
                        });
             */

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
 
    }
}
