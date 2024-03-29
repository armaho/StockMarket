﻿namespace Lib;

public class Trade
{
    static public int NextInstanceId { private get; set; } = 0;

    public int Id { get; init; }
    public int BuyOrderId { get; init; }
    public int SellOrderId { get; init; }
    public decimal Price { get; init; }
    public decimal Quantity { get; init; }

    public Trade(int buyOrderId, int sellOrderId, decimal price, decimal quantity)
    {
        this.Id = NextInstanceId++;
        this.BuyOrderId = buyOrderId;
        this.SellOrderId = sellOrderId;
        this.Price = price;
        this.Quantity = quantity;
    }
}

