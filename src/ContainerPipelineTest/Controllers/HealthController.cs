using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ContainerPipelineTest.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Models.AppDbContext _dbContext;

        public HealthController(Models.AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        /// <summary>
        /// Checks database connectivity
        /// </summary>
        [HttpGet("health/database")]
        public async Task<IActionResult> HealthDatabase()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
                }
                else
                {
                    return StatusCode(503, new { Status = "Unhealthy", Timestamp = DateTime.UtcNow });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Error = ex.Message, Timestamp = DateTime.UtcNow });
            }
        }
        
    }
}
