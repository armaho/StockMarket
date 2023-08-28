using FluentAssertions;
using StockMarket.Domain;
using Lib;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;
using FluentAssertions.Equivalency;

namespace StockMarket.Data.Tests;

public class IntegrationTests : IClassFixture<StockMarketDbContextFixture>
{
    private readonly StockMarketDbContext context;

    public IntegrationTests(StockMarketDbContextFixture fixture)
    {
        context = fixture.Context;
    }

    [Fact]
    public void DbContext_Should_Save_Orders_In_Database_Test()
    {
        //Arrange
        var processor = new StockMarketProcessor(IsMarketOpen: true);

        Order.NextInstanceId = ((context.Orders.Max(order => (int?)order.Id)) ?? 0) + 1;

        var buyOrderId = processor.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1M);
        var sellOrderId = processor.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);

        var buyOrder = processor.Orders.First(order => (order.Id == buyOrderId));
        var sellOrder = processor.Orders.First(order => (order.Id == sellOrderId));

        //Act
        context.Orders.Add(buyOrder);
        context.Orders.Add(sellOrder);

        context.SaveChanges();

        //Assert
        context.Orders.First(odrer => (odrer.Id == buyOrderId)).Should().BeEquivalentTo(new
        {
            Id = buyOrderId,
            Side = TradeSide.Buy,
            Price = 1500M,
            Quantity = 0M,
            IsCanceled = false
        });
        context.Orders.First(odrer => (odrer.Id == sellOrderId)).Should().BeEquivalentTo(new
        {
            Id = sellOrderId,
            Side = TradeSide.Sell,
            Price = 1500M,
            Quantity = 0M,
            IsCanceled = false
        });
    }

    [Fact]
    public void DbContext_Should_Save_Trades_In_Database_Test()
    {
        //Arrange
        var processor = new StockMarketProcessor(IsMarketOpen: true);

        Order.NextInstanceId = ((context.Orders.Max(order => (int?)order.Id)) ?? -1) + 1;
        Trade.NextInstanceId = ((context.Trades.Max(trade => (int?)trade.Id)) ?? -1) + 1;

        var buyOrderId = processor.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1M);
        var sellOrderId = processor.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);

        var buyOrder = processor.Orders.First(order => (order.Id == buyOrderId));
        var sellOrder = processor.Orders.First(order => (order.Id == sellOrderId));
        var createdTrade = processor.Trades[0];

        //Act
        context.Orders.Add(buyOrder);
        context.Orders.Add(sellOrder);
        context.Trades.Add(createdTrade);
        context.SaveChanges();

        //Assert
        context.Trades.First(trade => (trade.Id == createdTrade.Id)).Should().BeEquivalentTo(new
        {
            Id = createdTrade.Id,
            SellOrderId = sellOrderId,
            BuyOrderId = buyOrderId,
            Price = 1500M,
            Quantity = 1M
        });
    }

    [Fact]
    public void DbContext_Should_Retrive_Orders_From_Database_Test()
    {
        //Arrange
        var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();
        optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=arman1383;Database=StockMarketTest");

        //Act
        var context1 = new StockMarketDbContext(optionsBuilder.Options);
        var processor1 = new StockMarketProcessor(IsMarketOpen: true);

        Order.NextInstanceId = ((context1.Orders.Max(order => (int?)order.Id)) ?? -1) + 1;
        Trade.NextInstanceId = ((context1.Trades.Max(trade => (int?)trade.Id)) ?? -1) + 1;

        var buyOrderId = processor1.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1M);
        var buyOrder = processor1.Orders.First(order => (order.Id == buyOrderId));

        context1.Orders.Add(buyOrder);

        context1.SaveChanges();
        context1.Dispose();

        var context2 = new StockMarketDbContext(optionsBuilder.Options);
        var processor2 = new StockMarketProcessor(
            IsMarketOpen: true,
            orders: context2.Orders.Where(order => ((order.Quantity > 0) && (!order.IsCanceled))).ToList()
        );

        var sellOrderId = processor2.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
        var sellOrder = processor2.Orders.First(order => (order.Id == sellOrderId));

        var trade = processor2.Trades[0];

        context2.Orders.Add(sellOrder);
        context2.Trades.Add(trade);

        context2.SaveChanges();
        context2.Dispose();

        var context3 = new StockMarketDbContext(optionsBuilder.Options);

        // Assert
        context3.Orders.First(order => (order.Id == buyOrderId)).Should().BeEquivalentTo(new
        {
            Id = buyOrderId,
            Side = TradeSide.Buy,
            Price = 1500M,
            Quantity = 0M,
            IsCanceled = false,
        });
        context3.Orders.First(order => (order.Id == sellOrderId)).Should().BeEquivalentTo(new
        {
            Id = sellOrderId,
            Side = TradeSide.Sell,
            Price = 1500M,
            Quantity = 0M,
            IsCanceled = false,
        });
        context3.Trades.First(trade => (trade.Id == trade.Id)).Should().BeEquivalentTo(new
        {
            Id = trade.Id,
            SellOrderId = sellOrderId,
            BuyOrderId = buyOrderId,
            Price = 1500M,
            Quantity = 1,
        });
    }
}
