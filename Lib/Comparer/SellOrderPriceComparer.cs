using Utilities;

namespace Lib.Comparer;

//Generaly, Sell Order with lower offered price have a better chance of being
//acctepted and should be given a higher priority in a PriorityQueue.
public class SellOrderPriceComparer : IComparer<decimal>
{
    private readonly static DecimalComparer decimalComparer = new DecimalComparer();

    public int Compare(decimal firstBuyOrderPrice, decimal secondBuyOrderPrice)
    {
        return decimalComparer.Compare(firstBuyOrderPrice, secondBuyOrderPrice);
    }
}

