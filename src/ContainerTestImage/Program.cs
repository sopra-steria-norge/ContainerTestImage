using Microsoft.Identity.Web;
using ContainerTestImage.Msal;
using ContainerTestImage.Database;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

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
                options.UseSqlServer(builder.Configuration.GetConnectionString("ContainerTestImageDatabase")));


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

            // // Apply migrations automatically at startup
            // using (var scope = app.Services.CreateScope())
            // {
            //     var dbContext = scope.ServiceProvider.GetRequiredService<ContainerTestImageContext>();
            //     dbContext.Database.Migrate();
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
