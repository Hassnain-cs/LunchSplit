using System.Globalization;
using System.Text;

namespace LunchSplit.Core;

public class ReceiptFormatter
{
    public ReceiptFormatter(string studentName)
    {
    }

    public string Format(Bill bill, List<Attendee> attendees, List<Share> roundedShares)
    {
        var sb = new StringBuilder();

        sb.AppendLine("LunchSplit Receipt");
        sb.AppendLine($"Subtotal: {Money(bill.Subtotal)}");
        sb.AppendLine($"Tax: {Money(bill.Tax)}");
        sb.AppendLine($"Tip: {Money(bill.Tip)}");
        sb.AppendLine($"Grand Total: {Money(bill.GrandTotal)}");

        foreach (Share share in roundedShares)
        {
            sb.AppendLine($"{share.Name}: {Money(share.Amount)}");
        }

        return sb.ToString();
    }

    private static string Money(decimal value)
        => "$" + value.ToString("0.00", CultureInfo.InvariantCulture);
}