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

            result = PaymentResult.Failure(
                firstError?.Code ?? "UNKNOWN_ERROR",
                firstError?.Detail ?? ex.Message);
        }

        return result;
    }

    //
    // FUNCTION    : GetTransactionStatusAsync
    // DESCRIPTION : Not built yet. It is added in test M3-10.
    // PARAMETERS  : string transactionId : id of the earlier payment
    // RETURNS     : Task<PaymentStatus> : never returns for now
    //
    public Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        throw new NotImplementedException("Built in M3-10.");
    }
}