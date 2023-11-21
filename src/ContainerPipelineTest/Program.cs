using Microsoft.Identity.Web;
using ContainerPipelineTest.Msal;

namespace ContainerPipelineTest
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            app.UseHttpsRedirection();

            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            var clientId = builder.Configuration.GetSection("AzureAd")["ClientId"];
            var clientSecret = builder.Configuration.GetSection("AzureAd")["ClientSecret"];
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint($"/swagger/1.0/swagger.json", "ContainerPipelineTest API");
                c.OAuthClientId(clientId);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });            

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

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
            });

            services.InstallAzureAdAuthentication(configuration);

            // Match each Interface with the .First() implementation it finds.
            services.Scan(a => a.FromAssemblyOf<Program>().AddClasses(publicOnly: true).AsMatchingInterface());
        }
    }
}
