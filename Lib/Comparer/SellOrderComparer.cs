using System;

namespace Lib.Comparer;

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

