using ClinicalTrials.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrials.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ClinicalTrial> ClinicalTrials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
