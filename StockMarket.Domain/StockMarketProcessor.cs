using System.Diagnostics;
using System.Drawing;
using Lib;

namespace StockMarket.Domain;

public class StockMarketProcessor : IStockMarketProcessor
{
    private List<Order> _orders;
    private List<Trade> _trades;
    private MarketState.MarketState _currentMarketState; //Methods that can be changed in different market states (open or closed) use the implementation in CurrentMarketState

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
    public StockMarketProcessor(bool IsMarketOpen = false, List<Order>? orders = null)
    {
        _orders = new();
        _trades = new();

        BuyQueue = new(new Lib.Comparer.OrderComparer(new Lib.Comparer.BuyOrderPriceComparer()));
        SellQueue = new(new Lib.Comparer.OrderComparer(new Lib.Comparer.SellOrderPriceComparer()));

        _currentMarketState = (IsMarketOpen ? new MarketState.OpenState(this) : new MarketState.CloseState(this));

        foreach (var order in (orders ?? new()))
        {
            EnqueueOrderByMarketState(order);
        }
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
        return EnqueueOrder(new Order(side, price, quantity));
    }

    public int EnqueueOrder(Order order)
    {
        return _currentMarketState.EnqueueOrder(order);
    }

    //Function below should only be called by classes that inherit from MarketState.
    internal int EnqueueOrderByMarketState(Order newOrder)
    {
        PriorityQueue<Order, (decimal price, int id)> matchedOrderQueue = ((newOrder.Side == TradeSide.Buy) ? BuyQueue : SellQueue);

        _orders.Add(newOrder);
        matchedOrderQueue.Enqueue(newOrder, (newOrder.Price, newOrder.Id));

        while (IsTransactionPossible())
        {
            MakeTransaction();
        }

        return newOrder.Id;
    }

    //Function below should only be called by classes that inherit from MarketState.
    internal void CancelOrderByMarketState(int cancelledOrderId)
    {
        _orders.Find(order => (order.Id == cancelledOrderId))?.setAsCancelled();
    }

    //Function below should only be called by classes that inherit from MarketState.
    internal int ModifyOrderByMarketState(int modifiedOrderId, decimal price, decimal quantity)
    {
        Order? modifiedOrder = _orders.Find(order => (order.Id == modifiedOrderId));

        ArgumentNullException.ThrowIfNull(modifiedOrder);

        modifiedOrder.setAsCancelled();

        return EnqueueOrder(modifiedOrder.Side, price, quantity);
    }

    private void MakeTransaction()
    {
        Order bestBuyOrder = AvailablePeek(TradeSide.Buy)!, bestSellOrder = AvailablePeek(TradeSide.Sell)!;
        decimal transactedQuantity = Math.Min(bestBuyOrder.Quantity, bestSellOrder.Quantity);

        bestBuyOrder.reduceQuantity(transactedQuantity);
        bestSellOrder.reduceQuantity(transactedQuantity);

        _trades.Add(new(bestBuyOrder.Id, bestSellOrder.Id, bestSellOrder.Price, transactedQuantity));
    }

    private bool IsTransactionPossible()
    {
        Order? buyOrderPeek = AvailablePeek(TradeSide.Buy), sellOrderPeek = AvailablePeek(TradeSide.Sell);

        if ((buyOrderPeek == null) || (sellOrderPeek == null))
        {
            return false;
        }

        return (buyOrderPeek.Price >= sellOrderPeek.Price);
    }

    //Returns first available order in order queue
    private Order? AvailablePeek(TradeSide tradeSide)
    {
        PriorityQueue<Order, (decimal price, int id)> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        while ((queue.Count > 0) && (queue.Peek().IsCanceled || (queue.Peek().Quantity == 0)))
        {
            queue.Dequeue();
        }

        return ((queue.Count == 0) ? null : queue.Peek());
    }
}
