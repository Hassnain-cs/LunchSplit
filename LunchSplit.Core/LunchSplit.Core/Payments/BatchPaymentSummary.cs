/*
 * FILE          : BatchPaymentSummary.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : BatchPaymentSummary
 *  PURPOSE : Holds the results of charging a whole group, and counts how
 *            many payments worked and how many did not.
 */
public record BatchPaymentSummary(IReadOnlyList<PaymentResult> Results)
{
    public int SuccessCount => Results.Count(result => result.IsSuccess);

    public int FailureCount => Results.Count(result => !result.IsSuccess);
}