using System;
using Lib;

namespace StockMarket.Domain.MarketState;

public class OpenState : MarketState
{
    public OpenState(StockMarketProcessor stockMarketProcessor)
    {
        this.stockMarketProcessor = stockMarketProcessor;
    }

    public override void CancelOrder(int cancelledOrderId)
    {
        stockMarketProcessor._orders.Find(order => (order.Id == cancelledOrderId))?.setAsCancelled();

        System.Console.WriteLine(stockMarketProcessor._orders.Find(order => (order.Id == cancelledOrderId))?.IsCanceled);
    }

    public override int EnqueueOrder(TradeSide side, decimal price, decimal quantity)
    {
        Order newOrder = new(side, price, quantity);
        PriorityQueue<Order, decimal> matchedOrderQueue = ((side == TradeSide.Buy) ? stockMarketProcessor.BuyQueue : stockMarketProcessor.SellQueue);

        stockMarketProcessor._orders.Add(newOrder);
        matchedOrderQueue.Enqueue(newOrder, newOrder.Price);

        while (stockMarketProcessor.IsTransactionPossible())
        {
            stockMarketProcessor.MakeTransaction();
        }

        return newOrder.Id;
    }

    public override int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
    {
        Order? modifiedOrder = stockMarketProcessor._orders.Find(order => (order.Id == modifiedOrderId));

        ArgumentNullException.ThrowIfNull(modifiedOrder);

        modifiedOrder.setAsCancelled();

        return EnqueueOrder(modifiedOrder.Side, price, quantity);
    }
}
