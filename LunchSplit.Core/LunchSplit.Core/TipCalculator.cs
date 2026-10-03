namespace LunchSplit.Core;

internal static class TipCalculator
{
    public static decimal Compute(decimal subtotal, TipMode mode, decimal tipInput)
    {
        if (subtotal < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal cannot be negative.");
        }

        if (mode == TipMode.Percent)
        {
            return subtotal * tipInput / 100m;
        }

        if (mode == TipMode.Fixed)
        {
            if (tipInput < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tipInput), "Fixed tip cannot be negative.");
            }

            return tipInput;
        }

        return 0m;
    }
}