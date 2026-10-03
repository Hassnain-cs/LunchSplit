using System.Globalization;
using System.Text;

namespace LunchSplit.Core;

public class ReceiptFormatter
{
    private readonly string _studentName;
    private readonly Func<DateTime> _clock;

    public ReceiptFormatter(string studentName, Func<DateTime>? clock = null)
    {
        _studentName = studentName;
        _clock = clock ?? (() => DateTime.Now);
    }

    public string Format(Bill bill, List<Attendee> attendees, List<Share> roundedShares)
    {
        DateTime now = _clock();
        var sb = new StringBuilder();

        sb.AppendLine("LunchSplit Receipt");
        sb.AppendLine($"Student: {_studentName}");
        sb.AppendLine($"Date: {now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"Created: {now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
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