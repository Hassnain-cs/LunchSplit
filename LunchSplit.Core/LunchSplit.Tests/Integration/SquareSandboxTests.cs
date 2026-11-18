using Microsoft.VisualStudio.TestTools.UnitTesting;
/*
 * FILE          : SquareSandboxTests.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-17
 */

using LunchSplit.Core.Payments;
using LunchSplit.Infrastructure;

namespace LunchSplit.Tests.Integration;

/*
 *  NAME    : SquareSandboxTests
 *  PURPOSE : Tier 1 tests. These talk to the REAL Square sandbox over the
 *            internet using test card nonces, so no real money moves. They
 *            need credentials in appsettings.local.json or in environment
 *            variables.
 */
[TestClass]
public class SquareSandboxTests
{
    // Test nonces from Square's sandbox testing docs.
    // TITLE        : "Payments API Sandbox testing - card nonces"
    // AUTHOR       : Square, Inc.
    // DATE         : 2026-11-17
    // AVAILABILITY : https://developer.squareup.com/docs/testing/test-values
    private const string VALID_NONCE = "cnon:card-nonce-ok";
    private const string DECLINED_NONCE = "cnon:card-nonce-declined";
    private const decimal TEST_AMOUNT = 1.00m;

    private static SquareSettings squareSettings = null!;
    private static SquarePaymentService paymentService = null!;

    //
    // FUNCTION    : SetUpSandbox
    // DESCRIPTION : Runs once before all tests here: loads settings, builds the service.
    // PARAMETERS  : TestContext context : supplied by MSTest, not used
    // RETURNS     : void
    //
    [ClassInitialize]
    public static void SetUpSandbox(TestContext context)
    {
        squareSettings = SquareSettings.Load();
        paymentService = new SquarePaymentService(squareSettings);
    }

    //
    // FUNCTION    : BuildRequest
    // DESCRIPTION : Makes a one-dollar request with the details the test gives it.
    // PARAMETERS  :
    //   string cardNonce       : the test card token
    //   string currency        : three-letter currency code
    //   string idempotencyKey  : unique key for this payment
    // RETURNS     : PaymentRequest : ready to send
    //
    private static PaymentRequest BuildRequest(
        string cardNonce,
        string currency,
        string idempotencyKey)
    {
        PaymentRequest request = new PaymentRequest(
            TEST_AMOUNT, currency, idempotencyKey, cardNonce);

        return request;
    }

    //
    // FUNCTION    : ProcessPayment_ValidNonce_ReturnsSuccessWithTransactionId
    // DESCRIPTION : A good test card must be approved and give us a transaction id.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-01")]
    public async Task ProcessPayment_ValidNonce_ReturnsSuccessWithTransactionId()
    {
        // Arrange
        PaymentRequest request = BuildRequest(
            VALID_NONCE, squareSettings.Currency, Guid.NewGuid().ToString());

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert: show Square's message if it fails, to make debugging easier
        Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId));
    }
}