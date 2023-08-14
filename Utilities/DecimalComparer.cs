namespace Utilities;

public class DecimalComparer : IComparer<decimal>
{
    public int Compare(decimal firstDecimal, decimal secondDecimal)
    {
        decimal diff = (decimal) (firstDecimal - secondDecimal);

        if (diff > 0)
            return 1;
        if (diff < 0)
            return -1;
        return 0;
    }
}