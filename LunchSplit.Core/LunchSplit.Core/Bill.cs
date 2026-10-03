namespace LunchSplit.Core;

public record Bill(decimal Subtotal, decimal Tax, TipMode TipMode, decimal TipInput);