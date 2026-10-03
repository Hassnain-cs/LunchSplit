namespace LunchSplit.Core;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (mode == RoundingMode.None)
        {
            return rawShares.ToList();
        }

        return rawShares
            .Select(s => s with { Amount = RoundToCents(s.Amount, mode) })
            .ToList();
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