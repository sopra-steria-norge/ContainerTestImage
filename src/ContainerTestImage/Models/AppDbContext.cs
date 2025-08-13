using Microsoft.EntityFrameworkCore;

namespace ContainerTestImage.Models
{
    public class SampleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SampleModel> SampleModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here
        }
    }
}
