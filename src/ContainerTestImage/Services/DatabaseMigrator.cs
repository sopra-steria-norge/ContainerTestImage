using ContainerTestImage.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerTestImage.Services
{
    public interface IDatabaseMigrator
    {
        Task MigrateDatabaseAsync();
        Task<bool> CanConnectToDatabaseAsync();
        Task<object> GetDatabaseInfoAsync();
    }

    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly ContainerTestImageContext _context;
        private readonly ILogger<DatabaseMigrator> _logger;

        public DatabaseMigrator(ContainerTestImageContext context, ILogger<DatabaseMigrator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CanConnectToDatabaseAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database connection");
                return false;
            }
        }

        public async Task MigrateDatabaseAsync()
        {
            _logger.LogInformation("Starting database migration");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                await _context.Database.MigrateAsync();
                stopwatch.Stop();
                _logger.LogInformation("Database migration completed in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error during database migration after {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<object> GetDatabaseInfoAsync()
        {
            try
            {
                var canConnect = await CanConnectToDatabaseAsync();
                if (!canConnect)
                {
                    return new
                    {
                        Status = "Unhealthy",
                        Message = "Cannot connect to database",
                        DataSource = _context.GetConnectionStringDataSource(),
                        ConnectionAttemptTime = DateTime.UtcNow
                    };
                }

                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
                var sampleCount = await _context.Samples.CountAsync();

                return new
                {
                    Status = "Healthy",
                    Message = "Database connection successful",
                    DataSource = _context.GetConnectionStringDataSource(),
                    PendingMigrations = pendingMigrations.ToList(),
                    AppliedMigrations = appliedMigrations.ToList(),
                    PendingMigrationCount = pendingMigrations.Count(),
                    AppliedMigrationCount = appliedMigrations.Count(),
                    SampleCount = sampleCount,
                    ConnectionAttemptTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database information");
                throw;
            }
        }
    }
}
