namespace Lib;

public class Trade
{
    public int BuyOrderId { get; init; }
    public int SellOrderId { get; init; }
    public decimal Price { get; init; }
    public decimal Quantity { get; init; }

    public Trade(int buyOrderId, int sellOrderId, decimal price, decimal quantity)
    {
        this.BuyOrderId = buyOrderId;
        this.SellOrderId = sellOrderId;
        this.Price = price;
        this.Quantity = quantity;
    }
}

