using System;
using Lib;

namespace StockMarket.Domain;

internal interface IStockMarketProcessor
{
    public void CancelOrder(int cancelledOrderId);
    public int EnqueueOrder(Order order);
    public int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity);
}
