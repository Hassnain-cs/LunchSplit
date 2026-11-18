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

        SquareModels.CreatePaymentResponse response =
            await squareClient.PaymentsApi.CreatePaymentAsync(body);

        PaymentResult result = PaymentResult.Success(response.Payment.Id);

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