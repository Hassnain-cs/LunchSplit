/*
 * FILE          : PaymentManager.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : PaymentManager
 *  PURPOSE : The business rules around taking payments. It depends only on
 *            IPaymentProcessor, never on Square directly, which is what
 *            lets us test it with a mock.
 */
public class PaymentManager
{
    private readonly IPaymentProcessor paymentProcessor;
    private const decimal ZERO_AMOUNT = 0m;

    //
    // FUNCTION    : PaymentManager
    // DESCRIPTION : Stores the processor this manager will hand work to.
    // PARAMETERS  : IPaymentProcessor paymentProcessor : the gateway to use
    // RETURNS     : none (constructor)
    //
    public PaymentManager(IPaymentProcessor paymentProcessor)
    {
        this.paymentProcessor = paymentProcessor;
    }

    //
    // FUNCTION    : ProcessPaymentAsync
    // DESCRIPTION :
    //   Passes the payment to the processor. Zero or negative amounts are
    //   rejected here without calling the processor. If the network fails,
    //   the raw HttpRequestException is wrapped in our own timeout exception.
    // PARAMETERS  : PaymentRequest request : what to charge
    // RETURNS     : Task<PaymentResult> : the processor's result, or a failure
    //
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        PaymentResult result;

        // Rule: money must be positive. Check it here so a bad request
        // never costs us a call to the gateway.
        if (request.Amount <= ZERO_AMOUNT)
        {
            result = PaymentResult.Failure(
                "INVALID_AMOUNT", "Payment amount must be greater than zero.");
        }
        else
        {
            try
            {
                result = await paymentProcessor.ProcessPaymentAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw new PaymentGatewayTimeoutException(
                    "The payment gateway could not be reached in time.", ex);
            }
        }

        return result;
    }

    //
    // FUNCTION    : ProcessBatchAsync
    // DESCRIPTION :
    //   Charges every request in the list, one at a time, and collects
    //   all the results. One failure does not stop the others.
    // PARAMETERS  : IEnumerable<PaymentRequest> requests : the payments to take
    // RETURNS     : Task<BatchPaymentSummary> : every result plus the counts
    //
    public async Task<BatchPaymentSummary> ProcessBatchAsync(IEnumerable<PaymentRequest> requests)
    {
        List<PaymentResult> results = new List<PaymentResult>();

        foreach (PaymentRequest request in requests)
        {
            PaymentResult result = await ProcessPaymentAsync(request);
            results.Add(result);
        }

        BatchPaymentSummary summary = new BatchPaymentSummary(results);

        return summary;
    }
}