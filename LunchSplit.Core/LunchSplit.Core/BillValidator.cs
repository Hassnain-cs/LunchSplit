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

        if (bill.Tax < 0)
        {
            errors.Add("Tax cannot be negative. Please enter 0 or more.");
        }

        if (attendees is null || attendees.Count == 0)
        {
            errors.Add("At least one attendee is required.");
        }
        else if (attendees.Any(a => a.Weight < 0))
        {
            errors.Add("Attendee weights cannot be negative.");
        }

        return new ValidationResult(errors);
    }
}