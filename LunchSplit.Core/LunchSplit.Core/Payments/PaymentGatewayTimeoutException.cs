/*
 * FILE          : PaymentGatewayTimeoutException.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : PaymentGatewayTimeoutException
 *  PURPOSE : Thrown when the payment gateway cannot be reached in time.
 *            The app can catch this one type and tell the user to retry.
 */
public class PaymentGatewayTimeoutException : Exception
{
    //
    // FUNCTION    : PaymentGatewayTimeoutException
    // DESCRIPTION : Builds the exception and keeps the original error inside.
    // PARAMETERS  :
    //   string message          : what went wrong, in plain words
    //   Exception innerException: the original low-level error
    // RETURNS     : none (constructor)
    //
    public PaymentGatewayTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}