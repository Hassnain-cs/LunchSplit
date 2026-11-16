using Microsoft.VisualStudio.TestTools.UnitTesting;
/*
 * FILE          : PaymentProcessorMoqTests.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-12
 */

using LunchSplit.Core.Payments;
using Moq;

namespace LunchSplit.Tests.Unit;

/*
 *  NAME    : PaymentProcessorMoqTests
 *  PURPOSE : Tier 2 tests. These check PaymentManager using a fake
 *            (mock) IPaymentProcessor, so no internet and no Square
 *            account are needed. Each test checks one rule about how
 *            the manager talks to the processor.
 */
[TestClass]
public class PaymentProcessorMoqTests
{
    // The nonce below comes from Square's sandbox testing docs.
    // TITLE        : "Payments API Sandbox testing - card nonces"
    // AUTHOR       : Square, Inc.
    // DATE         : 2026-11-15
    // AVAILABILITY : https://developer.squareup.com/docs/testing/test-values
    private const string VALID_NONCE = "cnon:card-nonce-ok";
    private const decimal SHARE_AMOUNT = 25.00m;
    private const int ATTENDEE_COUNT = 4;
    private const string DECLINED_KEY = "key-2";

    //
    // FUNCTION    : SampleRequest
    // DESCRIPTION :
    //   Builds a normal-looking payment request so each test does not
    //   have to repeat the same four lines.
    // PARAMETERS  :
    //   decimal amount         : dollars to charge
    //   string idempotencyKey  : unique key for this payment
    // RETURNS     :
    //   PaymentRequest : a ready-to-use request
    //
    private static PaymentRequest SampleRequest(
        decimal amount = SHARE_AMOUNT,
        string idempotencyKey = "key-001")
    {
        PaymentRequest request = new PaymentRequest(amount, "CAD", idempotencyKey, VALID_NONCE);

        return request;
    }

    //
    // FUNCTION    : ProcessPayment_ValidRequest_InvokesProcessorWithExactAmount
    // DESCRIPTION :
    //   A good request must reach the processor exactly once, carrying
    //   the exact same amount we gave the manager.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-04")]
    public async Task ProcessPayment_ValidRequest_InvokesProcessorWithExactAmount()
    {
        // Arrange: a fake processor that happily approves anything
        Mock<IPaymentProcessor> fakeProcessor = new Mock<IPaymentProcessor>();
        fakeProcessor
            .Setup(processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(PaymentResult.Success("tx-001"));
        PaymentManager manager = new PaymentManager(fakeProcessor.Object);

        // Act
        PaymentResult result = await manager.ProcessPaymentAsync(SampleRequest());

        // Assert: it worked, and the processor saw exactly $25.00 once
        Assert.IsTrue(result.IsSuccess);
        fakeProcessor.Verify(
            processor => processor.ProcessPaymentAsync(
                It.Is<PaymentRequest>(sent => sent.Amount == SHARE_AMOUNT)),
            Times.Once);
    }

    //
    // FUNCTION    : ProcessPayment_GatewayNetworkTimeout_ThrowsPaymentGatewayTimeoutException
    // DESCRIPTION :
    //   If the gateway cannot be reached, the manager must throw our own
    //   PaymentGatewayTimeoutException instead of leaking a raw network error.
    // PARAMETERS  : none
    // RETURNS     : Task : completes when the test finishes
    //
    [TestMethod]
    [TestCategory("M3-03")]
    public async Task ProcessPayment_GatewayNetworkTimeout_ThrowsPaymentGatewayTimeoutException()
    {
        // Arrange: the fake processor behaves as if the network died
        Mock<IPaymentProcessor> fakeProcessor = new Mock<IPaymentProcessor>();
        fakeProcessor
            .Setup(processor => processor.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ThrowsAsync(new HttpRequestException("simulated network timeout"));
        PaymentManager manager = new PaymentManager(fakeProcessor.Object);

        // Act
        PaymentGatewayTimeoutException thrown =
            await Assert.ThrowsExactlyAsync<PaymentGatewayTimeoutException>(
                async () => await manager.ProcessPaymentAsync(SampleRequest()));

        // Assert: the original error is kept inside for debugging
        Assert.IsNotNull(thrown.InnerException);
    }
}