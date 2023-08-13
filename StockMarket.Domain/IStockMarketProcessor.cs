using System;
using Lib;

namespace StockMarket.Domain;

internal interface IStockMarketProcessor
{
    public void CancelOrder(int cancelledOrderId);
    public int EnqueueOrder(TradeSide side, decimal price, decimal quantity);
    public int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity);
}
