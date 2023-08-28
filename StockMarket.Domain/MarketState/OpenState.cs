using System;
using Lib;

namespace StockMarket.Domain.MarketState;

public class OpenState : MarketState
{
    public OpenState(StockMarketProcessor stockMarketProcessor) : base(stockMarketProcessor) { }

    public override void CancelOrder(int cancelledOrderId)
    {
        stockMarketProcessor.CancelOrderByMarketState(cancelledOrderId);
    }

    public override int EnqueueOrder(Order order)
    {
        return stockMarketProcessor.EnqueueOrderByMarketState(order);
    }

    public override int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
    {
        return stockMarketProcessor.ModifyOrderByMarketState(modifiedOrderId, price, quantity);
    }
}
