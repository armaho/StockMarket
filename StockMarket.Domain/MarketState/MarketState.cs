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

        public virtual int EnqueueOrder(TradeSide side, decimal price, decimal quantity)
        {
            throw new NotImplementedException();
        }

        public virtual int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
        {
            throw new NotImplementedException();
        }

        public virtual void CancelOrder(int cancelledOrderId)
        {
            throw new NotImplementedException();
        }
    }
}

