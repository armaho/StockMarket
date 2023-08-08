using Lib;
using FluentAssertions;

namespace StockMarket.Domain.Tests
{
    public class TestStockMarketProccessor : StockMarketProcessor
    {
    }

    public class StockMarketProcessorTests
    {
        private readonly TestStockMarketProccessor sut;

        public StockMarketProcessorTests()
        {
            sut = new();
        }

        [Fact]
        public void EnqueueOrder_should_process_sellOrder_when_buyOrder_is_already_enqueued_test()
        {
            // Arrange
            // Act
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            var sellOrderId = sut.EnqueueOrder(TradeSide.Sell, 1400M, 2M);
            // Assert
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Price = 1400M,
                Quantity = 1M
            });
        }

        [Fact]
        public void EnqueueOrder_should_process_buyOrder_when_sellOrder_is_already_enqueued_Test()
        {
            // Arrange
            // Act
            var sellOrderId = sut.EnqueueOrder(TradeSide.Sell, 1400M, 2M);
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            // Assert
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Price = 1400M,
                Quantity = 1M
            });
        }

        [Fact]
        public void EnqueueOrder_should_process_sellOrder_when_multiple_buyOrders_are_already_enqueued_test()
        {
            // Arrange
            // Act
            var buyOrderId1 = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            var buyOrderId2 = sut.EnqueueOrder(TradeSide.Buy, 1450M, 1M);
            var sellOrderId = sut.EnqueueOrder(TradeSide.Sell, 1400M, 2M);
            // Assert
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Price = 1400M,
                Quantity = 1M
            });
        }

        [Fact]
        public void EnqueueOrder_should_process_buyOrder_when_multiple_sellOrders_are_already_enqueued_test()
        {
            // Arrange
            // Act
            var sellOrderId1 = sut.EnqueueOrder(TradeSide.Sell, 1400M, 1M);
            var sellOrderId2 = sut.EnqueueOrder(TradeSide.Sell, 1400M, 1M);
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 2M);
            // Assert
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId1,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId2,
                Price = 1400M,
                Quantity = 1M
            });
        }

        [Fact]
        public void CancelOrder_should_cancel_the_order_test()
        {
            // Arrange
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            // Act
            sut.CancelOrder(buyOrderId);
            // Assert
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Id = buyOrderId,
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 1M,
                IsCanceled = true,
            });
        }

        [Fact]
        public void CancelOrder_should_dequeue_the_order_test()
        {
            // Arrange
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            // Act
            sut.CancelOrder(buyOrderId);
            sut.EnqueueOrder(TradeSide.Sell, 1400M, 2M);
            // Assert
            Assert.Empty(sut.Trades);
        }

        [Fact]
        public void ModifyOrder_should_update_order_test()
        {
            // Arrange
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, 1500M, 1M);
            var sellOrderId = sut.EnqueueOrder(TradeSide.Sell, 1600M, 2M);
            // Act
            var modifiedSellOrderId = sut.ModifyOrder(sellOrderId, 1400M, 1M);
            // Assert
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = modifiedSellOrderId,
                Price = 1400M,
                Quantity = 1M
            });
        }
    }
}