namespace LunchSplit.Core;

public record Bill(decimal Subtotal, decimal Tax, TipMode TipMode, decimal TipInput)
{
    public decimal Tip => TipCalculator.Compute(Subtotal, TipMode, TipInput);

    public decimal GrandTotal => Subtotal + Tax + Tip;
}