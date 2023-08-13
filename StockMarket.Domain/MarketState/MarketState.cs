using System;
using Lib;

namespace StockMarket.Domain.MarketState
{
	public abstract class MarketState : IStockMarketProcessor
	{
        protected StockMarketProcessor stockMarketProcessor;

        public abstract int EnqueueOrder(TradeSide side, decimal price, decimal quantity);
        public abstract int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity);
        public abstract void CancelOrder(int cancelledOrderId);
    }
}

