using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace StockMarket.Data;

internal class StockMarketDbContextFactory : IDesignTimeDbContextFactory<StockMarketDbContext>
{
    public StockMarketDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();

        optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=arman1383;Database=StockMarket");

        return new StockMarketDbContext(optionsBuilder.Options);
    }
}
