using Lib;
using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data;

public class StockMarketDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public StockMarketDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>(builder =>
            {
                builder.Property(order => order.Id).ValueGeneratedNever();
                builder.Property(order => order.Side);
            }
        );
    }
}
