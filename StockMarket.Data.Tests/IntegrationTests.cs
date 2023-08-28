using FluentAssertions;
using StockMarket.Domain;
using Lib;
using Microsoft.EntityFrameworkCore;

namespace StockMarket.Data.Tests;

public class IntegrationTests
{
    [Fact]
    public void DbContext_Should_Save_Orders_In_Database_Test()
    {
        //Arrange
        var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();

        optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=arman1383;Database=StockMarket");

        using var context = new StockMarketDbContext(optionsBuilder.Options);
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
        var optionsBuilder = new DbContextOptionsBuilder<StockMarketDbContext>();

        optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=arman1383;Database=StockMarket");

        using var context = new StockMarketDbContext(optionsBuilder.Options);
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
}