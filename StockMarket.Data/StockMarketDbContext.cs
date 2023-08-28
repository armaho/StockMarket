using Lib;
using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data;

public class StockMarketDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Trade> Trades { get; set; }

    public StockMarketDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>(builder =>
            {
                builder.Property(order => order.Id).ValueGeneratedNever();
                builder.Property(order => order.Side);
            }
        );
        builder.Entity<Trade>(builder =>
            {
                builder.Property(trade => trade.Id).ValueGeneratedNever();
                builder.Property(trade => trade.BuyOrderId);
                builder.Property(trade => trade.SellOrderId);
                builder.Property(trade => trade.Price);
                builder.Property(trade => trade.Quantity);
            }
        );
    }
}
