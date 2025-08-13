using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ContainerTestImage.Database;
using Microsoft.Extensions.DependencyInjection;

namespace ContainerTestImage.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ContainerTestImageContext _context;

        public HealthController(ContainerTestImageContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Health endpoint to check if the service is running. Required for the API gateway to work.
        /// </summary>
        /// <returns></returns>
        [HttpGet("health")]
        public string Health()
        {
            return "OK";
        }

        /// <summary>
        /// Health endpoint to check if the service is running. Required for the API gateway to work.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        public string HealthRoot()
        {
            return "OK";
        }


        [Authorize]
        [HttpGet("health/authorize")]
        public string HealthAuthorize()
        {
            return "OK";
        }

        // [Authorize]
        [HttpGet("health/database")]
        public string HealthDatabase()
        {
            try
            {
                // Test database connection
                _context.Database.CanConnect();

                return "OK";
            }
            catch (Exception)
            {
                throw new Exception($"Tjenesten klarer ikke å koble seg til databasen: {_context.GetConnectionStringDataSource()}");
            }
        }

        /// <summary>
        /// Health endpoint to check if the on-prem service is reachable.
        /// Calls the OnPrem service: https://demo.int/asdf/ping and returns the response.
        /// </summary>
        /// <returns>A string indicating the health status.</returns>
        [HttpGet("health/onprem")]
        public async Task<IActionResult> HealthOnPrem()
        {
            // For development only: bypass certificate validation.
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            using (var httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync("https://demo.int/asdf/ping");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return this.Content(content, "application/json");
                }
                else
                {
                    return this.StatusCode((int)response.StatusCode, $"Failed to reach the service: {response.StatusCode}");
                }
            }
        }


        [Authorize]
        [HttpGet("debug/migratedatabase")]
        public string Migratedatabase()
        {
            _context.MigrateDatabase();
            return "OK";
        }
    }
}

