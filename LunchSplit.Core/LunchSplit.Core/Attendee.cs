namespace LunchSplit.Core;

public record Attendee(string Name, int Weight = 1, bool Included = true);