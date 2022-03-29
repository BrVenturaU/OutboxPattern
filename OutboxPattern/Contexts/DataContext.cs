using Microsoft.EntityFrameworkCore;
using OutboxPattern.Entities;
using OutboxPattern.Messages;

namespace OutboxPattern.Contexts
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant() { 
                    Id = 1,
                    Name = "Default"
                }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
