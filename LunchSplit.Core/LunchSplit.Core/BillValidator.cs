namespace LunchSplit.Core;

public class BillValidator
{
    public ValidationResult Validate(Bill bill, List<Attendee> attendees)
    {
        var errors = new List<string>();

        if (bill.Subtotal < 0)
        {
            errors.Add("Subtotal cannot be negative. Please enter 0 or more.");
        }

        if (attendees.Count == 0)
        {
            errors.Add("At least one attendee is required.");
        }

        return new ValidationResult(errors);
    }

}