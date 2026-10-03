namespace LunchSplit.Core;

public class Splitter
{
    private readonly Rounder _rounder = new();

    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
    {
        return TipCalculator.Compute(subtotal, mode, tipInput);
    }

    public List<Share> CalculateShares(Bill bill, List<Attendee> attendees, RoundingMode roundingMode)
    {
        List<Attendee> active = attendees.Where(a => a.Included).ToList();
        int totalWeight = active.Sum(a => a.Weight);
        decimal grandTotal = bill.GrandTotal;

        List<Share> rawShares = attendees
            .Select(a => new Share(a.Name, a.Included ? grandTotal * a.Weight / totalWeight : 0m))
            .ToList();

        return _rounder.RoundShares(rawShares, roundingMode);
    }


}