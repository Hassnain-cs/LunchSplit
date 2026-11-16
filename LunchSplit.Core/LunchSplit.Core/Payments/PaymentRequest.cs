/*
 * FILE          : PaymentRequest.cs
 * PROJECT       : CSCN72010 - Assignment 3
 * PROGRAMMER    : Muhammad Hassnain
 * FIRST VERSION : 2026-11-15
 */

namespace LunchSplit.Core.Payments;

/*
 *  NAME    : PaymentRequest
 *  PURPOSE : Everything needed to charge one person once: how much, in what
 *            currency, a unique key (so a retry cannot double-charge), and
 *            the card token (nonce). It is only data, with no behaviour.
 */
public record PaymentRequest(
    decimal Amount,
    string Currency,
    string IdempotencyKey,
    string CardNonce);