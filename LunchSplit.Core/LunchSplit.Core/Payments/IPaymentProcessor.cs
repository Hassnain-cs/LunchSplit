/*
 * FILE          : IPaymentProcessor.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : IPaymentProcessor
 *  PURPOSE : The contract any payment gateway must follow. The core logic
 *            only knows this interface, so Square can be swapped for a
 *            mock in tests, or for another company later.
 */
public interface IPaymentProcessor
{
    //
    // FUNCTION    : ProcessPaymentAsync
    // DESCRIPTION : Tries to charge the card described by the request.
    // PARAMETERS  : PaymentRequest request : what to charge
    // RETURNS     : Task<PaymentResult> : success or failure details
    //
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

    //
    // FUNCTION    : GetTransactionStatusAsync
    // DESCRIPTION : Looks up where an earlier payment currently stands.
    // PARAMETERS  : string transactionId : id returned by ProcessPaymentAsync
    // RETURNS     : Task<PaymentStatus> : the payment's current state
    //
    Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
}