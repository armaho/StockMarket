using System.Diagnostics;
using Lib;

namespace StockMarket.Domain;

public class StockMarketProcessor : IStockMarketProcessor
{
    public List<Order> Orders { get; init; }
    public List<Trade> Trades { get; init; }
    public PriorityQueue<Order, decimal> BuyQueue { internal get; init; }
    public PriorityQueue<Order, decimal> SellQueue { internal get; init; }

    //Methods that can be changed in different market states (open or closed) use
    //the implementation in CurrentMarketState
    private MarketState.MarketState CurrentMarketState;

    //When creating a new market, it's close by default. To make an open market,
    //parameter IsMarketOpen should be true
    public StockMarketProcessor(bool IsMarketOpen = false)
    {
        Orders = new();
        Trades = new();
        BuyQueue = new(new BuyOrderComparer());
        SellQueue = new(new SellOrderComparer());

        CurrentMarketState = (IsMarketOpen ? new MarketState.OpenState(this) : new MarketState.CloseState(this));
    }

    public void CloseMarket()
    {
        CurrentMarketState = new MarketState.CloseState(this);
    }

    public void OpenMarket()
    {
        CurrentMarketState = new MarketState.OpenState(this);
    }

    public void CancelOrder(int cancelledOrderId)
    {
        CurrentMarketState.CancelOrder(cancelledOrderId);
    }

    //WARNING: New Order doesn't have the same ID as the old one.
    public int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
    {
        return CurrentMarketState.ModifyOrder(modifiedOrderId, price, quantity);
    }

    //Adds new order to the matched queue
    public int EnqueueOrder(TradeSide side, decimal price, decimal quantity)
    {
        return CurrentMarketState.EnqueueOrder(side, price, quantity);
    }

    internal void MakeTransaction()
    {
        Order? bestBuyOrder = AvailablePeek(TradeSide.Buy), bestSellOrder = AvailablePeek(TradeSide.Sell);
        decimal transactedQuantity = Math.Min(bestBuyOrder.Quantity, bestSellOrder.Quantity);

        bestBuyOrder.reduceQuantity(transactedQuantity);
        bestSellOrder.reduceQuantity(transactedQuantity);

        Trades.Add(new(bestBuyOrder.Id, bestSellOrder.Id, bestSellOrder.Price, transactedQuantity));
    }

    internal bool IsTransactionPossible()
    {
        return ((AvailablePeek(TradeSide.Buy)?.Price ?? 0) >= (AvailablePeek(TradeSide.Sell)?.Price ?? decimal.MaxValue));
    }

    //Returns Peek Item after calling ClearQueue() 
    private Order? AvailablePeek(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        ClearQueue(tradeSide);

        return ((queue.Count == 0) ? null : queue.Peek());
    }

    //Since some orders could be cancelled or modified, or their quantity is simply 0,
    //this function removes the peek element of queue until there is an available order.
    private void ClearQueue(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        while ((queue.Count > 0) && queue.Peek().IsCanceled && (queue.Peek().Quantity == 0))
        {
            queue.Dequeue();
        }
    }
}
