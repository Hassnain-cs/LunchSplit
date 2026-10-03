namespace LunchSplit.Core;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (mode == RoundingMode.Bankers)
        {
            return rawShares
                .Select(s => s with { Amount = Math.Round(s.Amount, 2, MidpointRounding.ToEven) })
                .ToList();
        }

        return rawShares.ToList();
    }
}