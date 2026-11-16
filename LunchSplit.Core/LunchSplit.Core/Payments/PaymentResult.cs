/*
 * FILE          : PaymentResult.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : PaymentResult
 *  PURPOSE : The answer to "did the payment work?". On success it carries
 *            the transaction id; on failure it carries an error code and
 *            message. Use the two factory methods so the fields always
 *            match each other.
 */
public record PaymentResult(
    bool IsSuccess,
    string? TransactionId,
    string? ErrorCode,
    string? ErrorMessage)
{
    //
    // FUNCTION    : Success
    // DESCRIPTION : Builds the result for a payment that went through.
    // PARAMETERS  : string transactionId : the gateway's id for the payment
    // RETURNS     : PaymentResult : IsSuccess true, no error fields
    //
    public static PaymentResult Success(string transactionId)
    {
        PaymentResult result = new PaymentResult(true, transactionId, null, null);

        return result;
    }

    //
    // FUNCTION    : Failure
    // DESCRIPTION : Builds the result for a payment that did not go through.
    // PARAMETERS  :
    //   string errorCode    : short machine-readable reason
    //   string errorMessage : human-readable explanation
    // RETURNS     : PaymentResult : IsSuccess false, no transaction id
    //
    public static PaymentResult Failure(string errorCode, string errorMessage)
    {
        PaymentResult result = new PaymentResult(false, null, errorCode, errorMessage);

        return result;
    }
}