/*
 * FILE          : SquarePaymentService.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-17
 */

using LunchSplit.Core.Payments;
using SquareAuth = Square.Legacy.Authentication;
using SquareLegacy = Square.Legacy;
using SquareModels = Square.Legacy.Models;
using SquareErrors = Square.Legacy.Exceptions;

namespace LunchSplit.Infrastructure;

/*
 *  NAME    : SquarePaymentService
 *  PURPOSE : The only class in the project that knows Square exists. It
 *            implements IPaymentProcessor by translating our request into a
 *            Square API call and Square's reply back into our PaymentResult.
 */
public class SquarePaymentService : IPaymentProcessor
{
    private const decimal CENTS_PER_DOLLAR = 100m;
    private const string SQUARE_GENERIC_DECLINE = "GENERIC_DECLINE";
    private const string CARD_DECLINED = "CARD_DECLINED";

    private readonly SquareLegacy.SquareClient squareClient;
    private readonly SquareSettings squareSettings;

    //
    // FUNCTION    : SquarePaymentService
    // DESCRIPTION :
    //   Builds the Square client pointed at the SANDBOX, with the token and
    //   a request timeout taken from the settings.
    // PARAMETERS  : SquareSettings settings : token, location id, timeout
    // RETURNS     : none (constructor)
    //
    public SquarePaymentService(SquareSettings settings)
    {
        squareSettings = settings;

        SquareAuth.BearerAuthModel credentials =
            new SquareAuth.BearerAuthModel.Builder(settings.AccessToken).Build();

        squareClient = new SquareLegacy.SquareClient.Builder()
            .BearerAuthCredentials(credentials)
            .Environment(SquareLegacy.Environment.Sandbox)
            .HttpClientConfig(config => config.Timeout(TimeSpan.FromSeconds(settings.TimeoutSeconds)))
            .Build();
    }

    //
    // FUNCTION    : ProcessPaymentAsync
    // DESCRIPTION :
    //   Sends one payment to Square. Square wants whole cents, so dollars
    //   are converted first. The idempotency key is passed through so a
    //   retry cannot charge twice.
    // PARAMETERS  : PaymentRequest request : what to charge
    // RETURNS     : Task<PaymentResult> : success with Square's payment id
    //
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        long amountInCents = (long)Math.Round(
            request.Amount * CENTS_PER_DOLLAR, MidpointRounding.AwayFromZero);

        SquareModels.Money money = new SquareModels.Money.Builder()
            .Amount(amountInCents)
            .Currency(request.Currency)
            .Build();

        SquareModels.CreatePaymentRequest body =
            new SquareModels.CreatePaymentRequest.Builder(request.CardNonce, request.IdempotencyKey)
                .AmountMoney(money)
                .LocationId(squareSettings.LocationId)
                .Build();

        PaymentResult result;

        try
        {
            SquareModels.CreatePaymentResponse response =
                await squareClient.PaymentsApi.CreatePaymentAsync(body);

            result = PaymentResult.Success(response.Payment.Id);
        }
        catch (SquareErrors.ApiException ex)
        {
            // Square sends back a list of errors. We keep the first one,
            // because it is the main reason the payment failed.
            SquareModels.Error? firstError = ex.Errors?.FirstOrDefault();

            string squareCode = firstError?.Code ?? "UNKNOWN_ERROR";

            result = PaymentResult.Failure(
                MapErrorCode(squareCode),
                firstError?.Detail ?? ex.Message,
                ex.ResponseCode);
        }

        return result;
    }
    //
    // FUNCTION    : MapErrorCode
    // DESCRIPTION :
    //   Translates Square's own error word into our domain vocabulary, so
    //   the rest of the app only ever sees our codes. For the sandbox
    //   declined-card nonce Square answers GENERIC_DECLINE, which we
    //   report as CARD_DECLINED. Any other code is passed through as is.
    // PARAMETERS  : string squareCode : the error code Square returned
    // RETURNS     : string : the code our domain uses
    //
    private static string MapErrorCode(string squareCode)
    {
        string domainCode = squareCode;

        if (squareCode == SQUARE_GENERIC_DECLINE)
        {
            domainCode = CARD_DECLINED;
        }

        return domainCode;
    }
    //
    // FUNCTION    : GetTransactionStatusAsync
    // DESCRIPTION :
    //   Asks Square for a payment by id and translates Square's status
    //   word into our PaymentStatus list.
    // PARAMETERS  : string transactionId : id of the earlier payment
    // RETURNS     : Task<PaymentStatus> : our version of the status
    //
    public async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        SquareModels.GetPaymentResponse response =
            await squareClient.PaymentsApi.GetPaymentAsync(transactionId);

        string statusText = response.Payment.Status ?? string.Empty;

        PaymentStatus status = statusText switch
        {
            "COMPLETED" => PaymentStatus.COMPLETED,
            "APPROVED" => PaymentStatus.APPROVED,
            "PENDING" => PaymentStatus.PENDING,
            "CANCELED" => PaymentStatus.CANCELED,
            "FAILED" => PaymentStatus.FAILED,
            _ => PaymentStatus.UNKNOWN
        };

        return status;
    }
}