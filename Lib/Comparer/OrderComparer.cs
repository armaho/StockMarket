using System;

namespace Lib.Comparer;

//Compare two orders based on their price and id
public class OrderComparer : IComparer<(decimal price, int id)>
{
    private readonly IComparer<decimal> priceComparer;

    public OrderComparer(IComparer<decimal> priceComparer)
    {
        this.priceComparer = priceComparer;
    }

    public int Compare((decimal price, int id) firstOrder, (decimal price, int id) secondOrder)
    {
        int priceCompareResult = priceComparer.Compare(firstOrder.price, secondOrder.price);

        return ((priceCompareResult == 0) ? (firstOrder.id - secondOrder.id) : priceCompareResult);
    }
}
