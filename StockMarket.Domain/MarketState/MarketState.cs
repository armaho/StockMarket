using System;
using Lib;

namespace StockMarket.Domain.MarketState
{
	public abstract class MarketState : IStockMarketProcessor
	{
        protected StockMarketProcessor stockMarketProcessor;

        protected MarketState(StockMarketProcessor stockMarketProcessor)
        {
            this.stockMarketProcessor = stockMarketProcessor;
        }

        public abstract int EnqueueOrder(TradeSide side, decimal price, decimal quantity);
        public abstract int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity);
        public abstract void CancelOrder(int cancelledOrderId);
    }
}

