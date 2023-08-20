using Lib;

namespace StockMarket.Domain.MarketState;

public class CloseState : MarketState
{
    public CloseState(StockMarketProcessor stockMarketProcessor) : base(stockMarketProcessor) { }
}
