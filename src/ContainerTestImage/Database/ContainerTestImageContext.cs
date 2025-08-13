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
        }

        public string GetConnectionStringDataSource()
        {
            var connBuilder = new SqlConnectionStringBuilder(Database.GetConnectionString());
            return connBuilder.DataSource;
        }

        public void MigrateDatabase()
        {
            Database.Migrate();
        }
    }
}

