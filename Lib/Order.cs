namespace Lib;

public enum TradeSide
{
    Buy,
    Sell
}

public class Order
{
    static private int nextInstanceId = 0; //Used to assign an unique Id to each instance

    public bool IsCanceled { get; private set; }
    public bool IsModified { get; private set; }
    public int Id { get; init; }
    public decimal Price { get; private set; }
    public decimal Quantity { get; private set; }
    public TradeSide Side { get; init; }

    public Order()
    {
        this.Id = nextInstanceId++;
        this.IsCanceled = false;
        this.IsModified = false;
    }

    public Order(TradeSide side, decimal price, decimal quantity) : this()
    {
        this.Price = price;
        this.Quantity = quantity;
        this.Side = side;
    }

    public void reduceQuantity(decimal quantity)
    {
        Quantity -= quantity;
    }

    public void setAsCancelled()
    {
        IsCanceled = true;
    }

    public void setAsModified()
    {
        IsModified = true;
    }
}

//MAYBE-LATER: SellOrderComparer and BuyOrderComparer look very simular. Maybe
//there is a way to combin both of them in the same class.

//Generaly, Sell Order with lower offered price have a better chance of being
//acctepted and should be given a higher priority in a PriorityQueue.
public class SellOrderComparer : IComparer<decimal>
{
    public int Compare(decimal firstSellOrderPrice, decimal secondSellOrderPrice)
    {
        decimal priceDifference = firstSellOrderPrice - secondSellOrderPrice;

        if (priceDifference < 0)
        {
            return -1;
        }
        else if (priceDifference > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

//Generaly, Buy Order with higher offered price have a better chance of being
//acctepted and should be given a higher priority in a PriorityQueue.
public class BuyOrderComparer : IComparer<decimal>
{
    public int Compare(decimal firstBuyOrderPrice, decimal secondBuyOrderPrice)
    {
        decimal priceDifference = firstBuyOrderPrice - secondBuyOrderPrice;

        if (priceDifference < 0)
        {
            return 1;
        }
        else if (priceDifference > 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
