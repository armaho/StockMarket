using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data.Tests;

public class StockMarketDbContextFixture : IDisposable
{
    public StockMarketDbContext Context { get; init; }

    public StockMarketDbContextFixture()
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();

        optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=arman1383;Database=StockMarketTest");

        Context = new(optionsBuilder.Options);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}