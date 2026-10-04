using LunchSplit.Core;

var bill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
var attendees = new List<Attendee> { new("Alice", 2), new("Bob"), new("Cara") };

ValidationResult validation = new BillValidator().Validate(bill, attendees);
if (!validation.IsValid)
{
    foreach (string error in validation.Errors)
    {
        Console.WriteLine(error);
    }

    return;
}

List<Share> shares = new Splitter().CalculateShares(bill, attendees, RoundingMode.Bankers);
Console.WriteLine(new ReceiptFormatter("Muhammad Hassnain").Format(bill, attendees, shares));