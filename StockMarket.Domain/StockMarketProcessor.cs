using System.Diagnostics;
using Lib;

namespace StockMarket.Domain;

public class StockMarketProcessor : IStockMarketProcessor
{
    public List<Order> Orders { get; init; }
    public List<Trade> Trades { get; init; }
    public PriorityQueue<Order, decimal> BuyQueue { internal get; init; }
    public PriorityQueue<Order, decimal> SellQueue { internal get; init; }

    //Methods that can be changes in different market states (open or closed) use
    //the implementation in CurrentMarketState
    private MarketState.MarketState CurrentMarketState;

    //When creating a new market, it's open by default. To make a closed market,
    //parameter IsMarketOpen should be false
    public StockMarketProcessor(bool IsMarketOpen = true)
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

    //If the possibility of transaction is alrealdy assured, this function doesn't check that
    //by setting possibilityAssurance to true
    internal void MakeTransaction(bool possibilityAssurance = false)
    {
        if (!possibilityAssurance && !IsTransactionPossible())
        {
            return;
        }

        Order? bestBuyOrder = AvailablePeek(TradeSide.Buy), bestSellOrder = AvailablePeek(TradeSide.Sell);
        decimal transactedQuantity = Math.Min(bestBuyOrder.Quantity, bestSellOrder.Quantity);

        bestBuyOrder.reduceQuantity(transactedQuantity);
        bestSellOrder.reduceQuantity(transactedQuantity);

        if (bestBuyOrder.Quantity == 0)
        {
            BuyQueue.Dequeue();
        }
        if (bestSellOrder.Quantity == 0)
        {
            SellQueue.Dequeue();
        }

        Trades.Add(new(bestBuyOrder.Id, bestSellOrder.Id, bestSellOrder.Price, transactedQuantity));
    }

    //Since some orders could be cancelled or modified, this function removes the peek element
    //of queue until there is an available order.
    internal void ClearQueue(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        while ((queue.Count > 0) && queue.Peek().IsCanceled)
        {
            queue.Dequeue();
        }
    }

    internal bool IsTransactionPossible()
    {
        ClearQueue(TradeSide.Buy);
        ClearQueue(TradeSide.Sell);

        return ((SellQueue.Count > 0) && (BuyQueue.Count > 0) && (SellQueue.Peek().Price <= BuyQueue.Peek().Price));
    }

    //Since some orders could be cancelled or modeified, so there is a need for a function that
    //returns the first peek that's not cancelled.
    internal Order AvailablePeek(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        ClearQueue(tradeSide);

        return queue.Peek();
    }
}