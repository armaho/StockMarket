using FluentAssertions;
using FluentAssertions.Equivalency;
using Lib;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace StockMarket.Domain.Test
{
    public class StockMarketProcessorTest
    {
        StockMarketProcessor sut; // System Under Test

        public StockMarketProcessorTest()
        {
            sut = new StockMarketProcessor();
            sut.OpenMarket();
        }

        [Fact]
        public void EnqueueOrder_Should_Process_SellOrder_When_BuyOrder_Is_Already_Enqueued_Test()
        {
            //Arrange
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);

            //Act

            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, quantity: 2, price: 1400);

            //Assert

            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Quantity = 0M,
                Price = 1500M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Quantity = 1M,
                Price = 1400M
            });

            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                Quantity = 1M,
                Price = 1400M,
                SellOrderId = sellOrderId,
                BuyOrderId = buyOrderId
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_BuyOrder_When_SellOrder_Is_Already_Enqueued_Test()
        {
            //Arrange

            sut.EnqueueOrder(side: TradeSide.Sell, quantity: 1, price: 1400);

            //Act

            sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);

            //Assert

            Assert.Equal(2, sut.Orders.Count());
            Assert.Single(sut.Trades);
        }

        [Fact]
        public void EnqueueOrder_Should_Process_SellOrder_With_The_Highest_Price_BuyOrder_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 3, price: 1500);
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 3, price: 1600);

            //Act
            var sellOrderId = sut.EnqueueOrder(TradeSide.Sell, quantity: 2, price: 1400);

            //Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {

                Quantity = 2M,
                Price = 1400M,
                SellOrderId = sellOrderId,
                BuyOrderId = buyOrderId2

            });
        }

        [Fact]
        public void EnqueueOrder_Should_Process_BuyOrder_With_Lowest_Price_SellOrder_Test()
        {
            //Arrange
            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, quantity: 3, price: 1500);
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, quantity: 3, price: 1600);

            //Act
            var buyOrderId = sut.EnqueueOrder(TradeSide.Buy, quantity: 2, price: 1700);

            //Assert
            Assert.Equal(3, sut.Orders.Count());
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {

                Quantity = 2M,
                Price = 1500M,
                SellOrderId = sellOrderId1,
                BuyOrderId = buyOrderId

            });
        }

        [Fact]
        public void CancelOrder_Should_Cancel_Order_Test()
        {
            //Arrange
            var orderId = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);

            //Act
            sut.CancelOrder(orderId);

            //Assert
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Quantity = 1M,
                Price = 1500M,
                IsCanceled = true
            });
        }

        [Fact]
        public void EnqueueOrder_Should_Not_Process_Order_When_Order_Is_Canceled_Test()
        {
            //Arrange
            var canceledOrderId = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CancelOrder(canceledOrderId);

            //Act
            sut.EnqueueOrder(side: TradeSide.Sell, quantity: 1, price: 1500);

            //Assert
            Assert.Empty(sut.Trades);
        }

        [Fact]
        public void EnqueueOrder_Should_Not_Work_When_StockMarket_Is_Closed_Test()
        {
            //Arrange
            sut.CloseMarket();

            //Act
            void act() => sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);

            //Assert
            Assert.Throws<NotImplementedException>(act);
        }

        [Fact]
        public void CancelOrder_Should_Not_Work_When_Stockmarket_Is_Closed_Test()
        {
            //Arrange
            var order = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();

            //Act
            void act() => sut.CancelOrder(order);

            //Assert
            Assert.Throws<NotImplementedException>(act);
        }

        [Fact]
        public void Modify_Order_Price_And_Quantity_Test()
        {
            // buy order o sell order dare ke ba ham match nemishan baadesh
            // yeki ro taghir mide ke ba ham badesh trade konan :))

            //Arrange
            var orderId = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1600);
            var sellOrder = sut.EnqueueOrder(side: TradeSide.Sell, quantity: 1, price: 1700);

            //Act
            sut.ModifyOrder(orderId, quantity: 1, price: 1800);

            //Assert
            Assert.Equal(3, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Quantity = 1M,
                Price = 1600M,
                IsCanceled = true
            });

            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Quantity = 0M,
                Price = 1800M,
                IsCanceled = false
            });
        }

        [Fact]
        public void Modify_Order_Should_Not_Work_When_StockMarket_Is_Closed()
        {
            //Arrange
            var order = sut.EnqueueOrder(side: TradeSide.Buy, quantity: 1, price: 1500);
            sut.CloseMarket();

            //Act
            void act() => sut.ModifyOrder(order, quantity: 1, price: 1400);

            //Assert
            Assert.Throws<NotImplementedException>(act);

        }
    }
}