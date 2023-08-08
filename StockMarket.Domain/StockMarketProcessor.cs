using System.Diagnostics;
using Lib;

namespace StockMarket.Domain;

public class StockMarketProcessor
{
    public List<Order> Orders { get; init; }
    public List<Trade> Trades { get; init; }
    public PriorityQueue<Order, decimal> BuyQueue { private get; init; }
    public PriorityQueue<Order, decimal> SellQueue { private get; init; }

    public StockMarketProcessor()
    {
        Orders = new();
        Trades = new();
        BuyQueue = new(new BuyOrderComparer());
        SellQueue = new(new SellOrderComparer());
    }

    public void CancelOrder(int cancelledOrderId)
    {
        Orders.Find(order => (order.Id == cancelledOrderId))?.setAsCancelled();
    }

    //WARNING: New Order doesn't have the same ID as the old one.
    public int ModifyOrder(int modifiedOrderId, decimal newPrice, decimal newQuantity)
    {
        Order? modifiedOrder = Orders.Find(order => (order.Id == modifiedOrderId));

        ArgumentNullException.ThrowIfNull(modifiedOrder);

        modifiedOrder.setAsModified();

        return EnqueueOrder(modifiedOrder.Side, newPrice, newQuantity);
    }

    public int EnqueueOrder(TradeSide tradeSide, decimal price, decimal quantity)
    {
        Order newOrder = new(tradeSide, price, quantity);
        PriorityQueue<Order, decimal> matchedOrderQueue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        Orders.Add(newOrder);
        matchedOrderQueue.Enqueue(newOrder, newOrder.Price);

        while (IsTransactionPossible())
        {
            MakeTransaction(possibilityAssurance: true);
        }

        return newOrder.Id;
    }

    //If the possibility of transaction is alrealdy assured, this function doesn't check that
    //by setting possibilityAssurance to true
    private void MakeTransaction(bool possibilityAssurance = false)
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
    private void ClearQueue(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        while ((queue.Count > 0) && (queue.Peek().IsCanceled || queue.Peek().IsModified))
        {
            queue.Dequeue();
        }
    }

    private bool IsTransactionPossible()
    {
        ClearQueue(TradeSide.Buy);
        ClearQueue(TradeSide.Sell);

        return ((SellQueue.Count > 0) && (BuyQueue.Count > 0) && (SellQueue.Peek().Price <= BuyQueue.Peek().Price));
    }

    //Since some orders could be cancelled or modeified, so there is a need for a function that
    //returns the first peek that's not cancelled.
    private Order AvailablePeek(TradeSide tradeSide)
    {
        PriorityQueue<Order, decimal> queue = ((tradeSide == TradeSide.Buy) ? BuyQueue : SellQueue);

        ClearQueue(tradeSide);

        return queue.Peek();
    }
}