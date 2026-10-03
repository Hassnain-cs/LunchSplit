namespace LunchSplit.Core;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (mode == RoundingMode.None)
        {
            return rawShares.ToList();
        }

        List<Share> rounded = rawShares
            .Select(s => s with { Amount = RoundToCents(s.Amount, mode) })
            .ToList();

        decimal rawTotal = Math.Round(rawShares.Sum(s => s.Amount), 6);
        decimal targetTotal = RoundToCents(rawTotal, mode);
        decimal leftover = targetTotal - rounded.Sum(s => s.Amount);

        rounded[0] = rounded[0] with { Amount = rounded[0].Amount + leftover };

        return rounded;
    }

    private static decimal RoundToCents(decimal value, RoundingMode mode)
    {
        if (mode == RoundingMode.RoundUp)
        {
            return Math.Ceiling(value * 100m) / 100m;
        }

        if (mode == RoundingMode.RoundDown)
        {
            return Math.Floor(value * 100m) / 100m;
        }

        return Math.Round(value, 2, MidpointRounding.ToEven);
    }
}