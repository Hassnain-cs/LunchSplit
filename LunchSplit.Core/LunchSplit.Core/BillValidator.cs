namespace LunchSplit.Core;

public class BillValidator
{
    public ValidationResult Validate(Bill bill, List<Attendee> attendees)
    {
        return new ValidationResult(new List<string>());
    }

}