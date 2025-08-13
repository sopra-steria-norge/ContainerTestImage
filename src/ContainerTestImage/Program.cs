using Microsoft.Identity.Web;
using ContainerTestImage.Msal;
using ContainerTestImage.Database;
using ContainerTestImage.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Scrutor;

namespace ContainerTestImage
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            // Configure DbContext with SQL Server
            builder.Services.AddDbContext<ContainerTestImageContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("ContainerTestImage"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: builder.Configuration.GetValue<int>("Database:MaxRetryCount", 3),
                            maxRetryDelay: TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("Database:MaxRetryDelay", 5)),
                            errorNumbersToAdd: null);
                        
                        sqlOptions.CommandTimeout(builder.Configuration.GetValue<int>("Database:CommandTimeout", 30));
                        
                        // Configure additional SQL Server options if needed
                        if (builder.Configuration.GetValue<bool>("Database:EnableSensitiveDataLogging", false))
                        {
                            options.EnableSensitiveDataLogging();
                        }
                    }));


            var app = builder.Build();

            app.UseHttpsRedirection();

            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            var clientId = builder.Configuration.GetSection("AzureAd")["ClientId"];
            var clientSecret = builder.Configuration.GetSection("AzureAd")["ClientSecret"];
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint($"/swagger/1.0/swagger.json", "ContainerTestImage API");
                c.OAuthClientId(clientId);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Apply migrations automatically at startup
            // using (var scope = app.Services.CreateScope())
            // {
            //     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            //     try
            //     {
            //         logger.LogInformation("Applying database migrations on startup");
            //         var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
            //         migrator.MigrateDatabaseAsync().Wait();
            //         logger.LogInformation("Database migrations applied successfully");
            //     }
            //     catch (Exception ex)
            //     {
            //         logger.LogError(ex, "An error occurred while applying database migrations");
            //     }
            // }

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            // Application Insights
            services.AddApplicationInsightsTelemetry();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.HandleSameSiteCookieCompatibility();
            });

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
            services.AddSwaggerGen(c =>
            {
                c.InstallSwaggerAuthentication(configuration);
                c.DescribeAllParametersInCamelCase();
                c.EnableAnnotations(); // Enable Swagger annotations
            });

            services.InstallAzureAdAuthentication(configuration);

            // Match each Interface with the .First() implementation it finds.
            services.Scan(a => a.FromAssemblyOf<Program>().AddClasses(publicOnly: true).AsMatchingInterface());


        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
                    });
        }
    }
}
