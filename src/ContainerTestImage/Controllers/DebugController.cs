using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ContainerTestImage.Database;

namespace ContainerTestImage.Controllers
{
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly ContainerTestImageContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DebugController> _logger;

        public DebugController(ContainerTestImageContext context, IWebHostEnvironment environment, ILogger<DebugController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet("debug/database/migrate")]
        public IActionResult Migrate()
        {
            if (!_environment.IsDevelopment())
            {
                return BadRequest("This action is only available in the development environment.");
            }

            try
            {
                _context.MigrateDatabase();
                _logger.LogInformation("Database migration completed successfully.");
                return Ok("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during migration");
                return StatusCode(500, $"An error occurred during migration: {ex.Message}");
            }
        }
    }
}
