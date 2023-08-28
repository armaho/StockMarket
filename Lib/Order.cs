namespace Lib;

public class Order
{
    static public int NextInstanceId { private get; set; } = 0; //Used to assign a unique Id to each instance

    public bool IsCanceled { get; private set; }
    public int Id { get; init; }
    public decimal Price { get; private set; }
    public decimal Quantity { get; private set; }
    public TradeSide Side { get; init; }

    public Order()
    {
        this.Id = NextInstanceId++;
        this.IsCanceled = false;
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
}
