using System;
using Lib;

namespace StockMarket.Domain.MarketState;

public class CloseState : MarketState
{
    public CloseState(StockMarketProcessor stockMarketProcessor) : base(stockMarketProcessor) { }

    public override void CancelOrder(int cancelledOrderId)
    {
        throw new NotImplementedException();
    }

    public override int EnqueueOrder(TradeSide side, decimal price, decimal quantity)
    {
        throw new NotImplementedException();
    }

    public override int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
    {
        throw new NotImplementedException();
    }
}
