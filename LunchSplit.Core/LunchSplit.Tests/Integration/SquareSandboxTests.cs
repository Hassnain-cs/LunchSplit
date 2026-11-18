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

    //
    // FUNCTION    : ProcessPayment_DeclinedNonce_ReturnsCardDeclined
    // DESCRIPTION :
    //   A declined test card must come back as a normal failure result with
    //   a decline-type error code, and must not throw.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-02")]
    public async Task ProcessPayment_DeclinedNonce_ReturnsCardDeclined()
    {
        // Arrange
        PaymentRequest request = BuildRequest(
            DECLINED_NONCE, squareSettings.Currency, Guid.NewGuid().ToString());

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert: Square's exact code can be CARD_DECLINED or GENERIC_DECLINE,
        // so we check that it contains DECLINE
        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains(result.ErrorCode, "DECLINE");
    }

    //
    // FUNCTION    : ProcessPayment_UnsupportedCurrency_FailsWithHttp400
    // DESCRIPTION :
    //   Asking for a currency the sandbox account does not use (EUR) must
    //   fail, and the result must carry Square's HTTP status 400.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-08")]
    public async Task ProcessPayment_UnsupportedCurrency_FailsWithHttp400()
    {
        // Arrange
        PaymentRequest request = BuildRequest(
            VALID_NONCE, "EUR", Guid.NewGuid().ToString());

        // Act
        PaymentResult result = await paymentService.ProcessPaymentAsync(request);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(400, result.HttpStatusCode);
    }

    //
    // FUNCTION    : ProcessPayment_DuplicateIdempotencyKey_ReturnsOriginalTransactionId
    // DESCRIPTION :
    //   Sending the same request twice with the same key must NOT charge
    //   twice. Square hands back the original payment, so both calls give
    //   the same transaction id.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-09")]
    public async Task ProcessPayment_DuplicateIdempotencyKey_ReturnsOriginalTransactionId()
    {
        // Arrange: one key, used twice
        string sharedKey = Guid.NewGuid().ToString();
        PaymentRequest request = BuildRequest(VALID_NONCE, squareSettings.Currency, sharedKey);

        // Act
        PaymentResult firstResult = await paymentService.ProcessPaymentAsync(request);
        PaymentResult secondResult = await paymentService.ProcessPaymentAsync(request);

        // Assert
        Assert.IsTrue(firstResult.IsSuccess, firstResult.ErrorMessage);
        Assert.IsTrue(secondResult.IsSuccess, secondResult.ErrorMessage);
        Assert.AreEqual(firstResult.TransactionId, secondResult.TransactionId);
    }
}