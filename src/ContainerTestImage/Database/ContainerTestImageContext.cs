using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ContainerTestImage.Models;

namespace ContainerTestImage.Database
{

    // Define the context class
    public class ContainerTestImageContext : DbContext
    {
        // ContainerTestImage models
        public DbSet<Sample> Samples { get; set; }

        public ContainerTestImageContext() : base() { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


        // DEFAULT CONSTRUCTOR BY STARTUP
        public ContainerTestImageContext(DbContextOptions options) : base(options)
        {
            // Installere alle migreringer fra Migration-katalogen
            // Database.MigrateAsync().Wait();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints
            modelBuilder.Entity<Sample>(entity =>
            {
                entity.HasKey(e => e.SampleId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
        }

        public string GetConnectionStringDataSource()
        {
            var connBuilder = new SqlConnectionStringBuilder(Database.GetConnectionString());
            return connBuilder.DataSource;
        }

        public void MigrateDatabase()
        {
            Database.Migrate();
            SeedData();
        }

        private void SeedData()
        {
            // Only seed if there's no data in the Samples table
            if (!Samples.Any())
            {
                Samples.AddRange(
                    new Sample
                    {
                        Name = "First Sample",
                        Description = "This is the first sample record created automatically during migration",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    },
                    new Sample
                    {
                        Name = "Second Sample",
                        Description = "This is the second sample record created automatically during migration",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    }
                );
                SaveChanges();
            }
        }
    }
}

