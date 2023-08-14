using System.Diagnostics;
using Lib;

namespace StockMarket.Domain;

public class StockMarketProcessor : IStockMarketProcessor
{
    private MarketState.MarketState _currentMarketState; //Methods that can be changed in different market states (open or closed) use the implementation in CurrentMarketState

    internal List<Order> _orders;
    internal List<Trade> _trades;

    public PriorityQueue<Order, (decimal price, int id)> BuyQueue { internal get; init; }
    public PriorityQueue<Order, (decimal price, int id)> SellQueue { internal get; init; }

    //Only for testing purposes
    public IReadOnlyList<Order> Orders
    {
        get
        {
            return (IReadOnlyList<Order>)_orders;
        }
    }

    //Only for testing purposes
    public IReadOnlyList<Trade> Trades
    {
        get
        {
            return (IReadOnlyList<Trade>)_trades;
        }
    }

    //When creating a new market, it's close by default. To make an open market,
    //parameter IsMarketOpen should be true
    public StockMarketProcessor(bool IsMarketOpen = false)
    {
        _orders = new();
        _trades = new();

        BuyQueue = new(new Lib.Comparer.OrderComparer(new Lib.Comparer.BuyOrderPriceComparer()));
        SellQueue = new(new Lib.Comparer.OrderComparer(new Lib.Comparer.SellOrderPriceComparer()));

        _currentMarketState = (IsMarketOpen ? new MarketState.OpenState(this) : new MarketState.CloseState(this));
    }

    public void CloseMarket()
    {
        _currentMarketState = new MarketState.CloseState(this);
    }

    public void OpenMarket()
    {
        _currentMarketState = new MarketState.OpenState(this);
    }

    public void CancelOrder(int cancelledOrderId)
    {
        _currentMarketState.CancelOrder(cancelledOrderId);
    }

    //WARNING: Modified Order doesn't have the same ID as the old one.
    public int ModifyOrder(int modifiedOrderId, decimal price, decimal quantity)
    {
        return _currentMarketState.ModifyOrder(modifiedOrderId, price, quantity);
    }

    //Adds new order to the matched queue
    public int EnqueueOrder(TradeSide side, decimal price, decimal quantity)
    {
        return _currentMarketState.EnqueueOrder(side, price, quantity);
    }

    internal void MakeTransaction()
    {
        Order? bestBuyOrder = AvailablePeek(TradeSide.Buy), bestSellOrder = AvailablePeek(TradeSide.Sell);
        decimal transactedQuantity = Math.Min(bestBuyOrder.Quantity, bestSellOrder.Quantity);

        bestBuyOrder.reduceQuantity(transactedQuantity);
        bestSellOrder.reduceQuantity(transactedQuantity);

        _trades.Add(new(bestBuyOrder.Id, bestSellOrder.Id, bestSellOrder.Price, transactedQuantity));
    }

    internal bool IsTransactionPossible()
    {
        return ((AvailablePeek(TradeSide.Buy)?.Price ?? 0) >= (AvailablePeek(TradeSide.Sell)?.Price ?? decimal.MaxValue));
    }

    //Returns Peek Item after calling ClearQueue() 
    private Order? AvailablePeek(TradeSide tradeSide)
    {
        PriorityQueue<Order, (decimal price, int id)> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        ClearQueue(tradeSide);

        return ((queue.Count == 0) ? null : queue.Peek());
    }

    //Since some orders could be cancelled or modified, or their quantity is simply 0,
    //this function removes the peek element of queue until there is an available order.
    private void ClearQueue(TradeSide tradeSide)
    {
        PriorityQueue<Order, (decimal price, int id)> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        while ((queue.Count > 0) && (queue.Peek().IsCanceled || (queue.Peek().Quantity == 0)))
        {
            queue.Dequeue();
        }
    }
}
