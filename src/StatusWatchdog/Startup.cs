using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StatusWatchdog.Authentication;
using StatusWatchdog.Services;

namespace StatusWatchdog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddControllers();
            services.AddSingleton<ServicesManager>();
            services.AddSingleton<IncidentsManager>();
            services.AddSingleton<IncidentUpdatesManager>();
            services.AddSingleton<KeyValuesManager>();
            services.AddSingleton<MetricsManager>();
            services.AddSingleton<DowntimeCalculator>();
            services.AddDbContext<WatchdogContext>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = InMemoryKeyAuthenticationHandlerOptions.DefaultScheme;
                x.DefaultChallengeScheme = InMemoryKeyAuthenticationHandlerOptions.DefaultScheme;
            })
            .UseInMemoryKey();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "ServiceWatchdog API",
                    Version = "v1"
                });
                x.EnableAnnotations();
                x.AddSecurityDefinition("apikey", new OpenApiSecurityScheme()
                {
                    Description = "API key authentication",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer"
                });

                x.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceWatchdog API");
                });
                app.UseReDoc(x =>
                {
                    x.RoutePrefix = "docs";
                    x.DocumentTitle = "StatusWatchdog API Documentation";
                    x.SpecUrl("/swagger/v1/swagger.json");
                });
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
