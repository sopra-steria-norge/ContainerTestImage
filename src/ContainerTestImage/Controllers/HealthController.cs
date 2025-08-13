using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using ContainerTestImage.Database;
using ContainerTestImage.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Swashbuckle.AspNetCore.Annotations;

namespace ContainerTestImage.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ContainerTestImageContext _context;
        private readonly IDatabaseMigrator _databaseMigrator;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            ContainerTestImageContext context,
            IDatabaseMigrator databaseMigrator,
            ILogger<HealthController> logger)
        {
            _context = context;
            _databaseMigrator = databaseMigrator;
            _logger = logger;
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
        [SwaggerOperation(Summary = "Database health check", Description = "Returns the health status of the database connection")]
        public async Task<ActionResult<object>> HealthDatabase()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var canConnect = await _context.Database.CanConnectAsync();
                stopwatch.Stop();
                
                if (!canConnect)
                {
                    _logger.LogWarning("Database health check failed: Cannot connect to database");
                    return StatusCode(503, new
                    {
                        Status = "Unhealthy",
                        Message = "Cannot connect to database",
                        DataSource = _context.GetConnectionStringDataSource(),
                        Timestamp = DateTime.UtcNow,
                        ResponseTimeMs = stopwatch.ElapsedMilliseconds
                    });
                }

                // If we can connect, perform a simple query
                var sampleCount = await _context.Samples.CountAsync();
                
                return Ok(new
                {
                    Status = "Healthy",
                    Message = "Database connection successful",
                    DataSource = _context.GetConnectionStringDataSource(),
                    SampleRecordCount = sampleCount,
                    Timestamp = DateTime.UtcNow,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed with exception");
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Message = ex.Message,
                    DataSource = _context.GetConnectionStringDataSource(),
                    Timestamp = DateTime.UtcNow
                });
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
        
        [HttpGet("health/database/status")]
        [SwaggerOperation(Summary = "Database migration status", Description = "Returns the status of database migrations")]
        public async Task<ActionResult<object>> GetDatabaseStatus()
        {
            try
            {
                var status = await _databaseMigrator.GetDatabaseInfoAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve database migration status");
                return StatusCode(500, new
                {
                    Status = "Failed",
                    Message = ex.Message,
                    DataSource = _context.GetConnectionStringDataSource(),
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}

