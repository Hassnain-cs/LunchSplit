namespace LunchSplit.Core;

public class Splitter
{
    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
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

    public List<Share> CalculateShares(Bill bill, List<Attendee> attendees, RoundingMode roundingMode)
    {
        int totalWeight = attendees.Sum(a => a.Weight);

        return attendees
            .Select(a => new Share(a.Name, bill.Subtotal * a.Weight / totalWeight))
            .ToList();
    }
}