namespace LunchSplit.Core;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (rawShares.Count == 0)
        {
            return new List<Share>();
        }

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

        int pennies = (int)Math.Round(Math.Abs(leftover) / 0.01m);
        decimal step = leftover > 0 ? 0.01m : -0.01m;

        List<int> payers = Enumerable.Range(0, rounded.Count)
            .Where(i => rawShares[i].Amount > 0)
            .ToList();

        if (payers.Count == 0)
        {
            return rounded;
        }

        for (int i = 0; i < pennies; i++)
        {
            int index = payers[i % payers.Count];
            rounded[index] = rounded[index] with { Amount = rounded[index].Amount + step };
        }

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