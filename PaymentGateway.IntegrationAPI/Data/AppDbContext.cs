using Microsoft.EntityFrameworkCore;
using PaymentGateway.IntegrationAPI.Models;

namespace PaymentGateway.IntegrationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
            .Property(o => o.Amount)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
            .HasMany(o => o.Transactions)
            .WithOne(t => t.Order)
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
              .Property(o => o.Status)
              .HasConversion<string>(); 

        }
    }
}
