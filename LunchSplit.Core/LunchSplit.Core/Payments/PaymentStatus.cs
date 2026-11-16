/*
 * FILE          : PaymentStatus.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : PaymentStatus
 *  PURPOSE : Our own list of payment states, so the rest of the app never
 *            has to know the raw words a payment company uses.
 */
public enum PaymentStatus
{
    UNKNOWN,
    PENDING,
    APPROVED,
    COMPLETED,
    CANCELED,
    FAILED
}