using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using trackingapi.Models;

namespace trackingapi.Data
{
    public class IssueDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public IssueDbContext(DbContextOptions<IssueDbContext> options, IConfiguration configuration)
            :base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}
