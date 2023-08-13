using System;

namespace Lib.Comparer;

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
