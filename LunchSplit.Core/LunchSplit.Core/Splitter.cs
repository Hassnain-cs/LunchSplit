namespace LunchSplit.Core;

public class Splitter
{
    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
    {
        if (mode == TipMode.Percent)
        {
            return subtotal * tipInput / 100m;
        }

        if (mode == TipMode.Fixed)
        {
            return tipInput;
        }

        return 0m;
    }
}