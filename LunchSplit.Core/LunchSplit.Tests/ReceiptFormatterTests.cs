using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class ReceiptFormatterTests
{
    [TestMethod]
    public void Format_DefaultBillReceipt_ContainsLineItems()
    {
        // Arrange
        var formatter = new ReceiptFormatter("Your Name");
        var bill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
        var attendees = new List<Attendee> { new("Alice"), new("Bob") };
        var shares = new List<Share> { new("Alice", 64.00m), new("Bob", 64.00m) };

        // Act
        string receipt = formatter.Format(bill, attendees, shares);

        // Assert
        Assert.IsTrue(receipt.Contains("Alice: $64.00"));
        Assert.IsTrue(receipt.Contains("Bob: $64.00"));
        Assert.IsTrue(receipt.Contains("Grand Total: $128.00"));
    }

    [TestMethod]
    public void Format_ReceiptOutput_ContainsRequiredMetadata()
    {
        // Arrange
        var fixedClock = new DateTime(2026, 9, 30, 14, 30, 0);
        var formatter = new ReceiptFormatter("Muhammad Hassnain", () => fixedClock);
        var bill = new Bill(100.00m, 13.00m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice") };
        var shares = new List<Share> { new("Alice", 113.00m) };

        // Act
        string receipt = formatter.Format(bill, attendees, shares);

        // Assert
        Assert.IsTrue(receipt.Contains("Student: Muhammad Hassnain"));
        Assert.IsTrue(receipt.Contains("Date: 2026-09-30"));
        Assert.IsTrue(receipt.Contains("Created: 2026-09-30 14:30:00"));
    }


    [TestMethod]
    [TestCategory("RED")]
    public void Format_CsvReceiptExporter_MatchesSchema()
    {
        // Arrange: the CSV exporter (Feature F5) does not exist yet.

        // Act: nothing to run until F5 is built.

        // Assert
        Assert.Fail("RED/PENDING: CSV receipt exporter (Feature F5) is not implemented yet.");
    }
}