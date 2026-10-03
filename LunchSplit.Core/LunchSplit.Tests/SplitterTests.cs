using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterTests
{
    [TestMethod]
    public void ComputeTip_NoTipMode_ReturnsZero()
    {
        // Arrange
        var splitter = new Splitter();

        // Act
        decimal tip = splitter.ComputeTip(100.00m, TipMode.None, 0m);

        // Assert
        Assert.AreEqual(0.00m, tip);
    }


    [TestMethod]
    public void ComputeTip_PercentTipMode_ReturnsCorrectPercent()
    {
        // Arrange
        var splitter = new Splitter();

        // Act
        decimal tip = splitter.ComputeTip(120.00m, TipMode.Percent, 15m);

        // Assert
        Assert.AreEqual(18.00m, tip);
    }


    [TestMethod]
    public void ComputeTip_FixedTipMode_ReturnsFixedAmount()
    {
        // Arrange
        var splitter = new Splitter();

        // Act
        decimal tip = splitter.ComputeTip(100.00m, TipMode.Fixed, 15.00m);

        // Assert
        Assert.AreEqual(15.00m, tip);
    }



    [TestMethod]
    public void ComputeTip_NegativeSubtotal_ThrowsException()
    {
        // Arrange
        var splitter = new Splitter();

        // Act + Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => splitter.ComputeTip(-1.00m, TipMode.Percent, 15m));
    }


    [TestMethod]
    public void ComputeTip_NegativeFixedTip_ThrowsException()
    {
        // Arrange
        var splitter = new Splitter();

        // Act + Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => splitter.ComputeTip(100.00m, TipMode.Fixed, -5.00m));
    }

    [TestMethod]
    public void CalculateShares_EqualSplit_ApportionsEvenly()
    {
        // Arrange
        var splitter = new Splitter();
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice"), new("Bob"), new("Cara") };

        // Act
        List<Share> shares = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        // Assert
        Assert.AreEqual(30.00m, shares[0].Amount);
        Assert.AreEqual(30.00m, shares[1].Amount);
        Assert.AreEqual(30.00m, shares[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_ProportionalSplit_ApportionsWeighted()
    {
        // Arrange
        var splitter = new Splitter();
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice", 2), new("Bob", 1) };

        // Act
        List<Share> shares = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        // Assert
        Assert.AreEqual(60.00m, shares[0].Amount);
        Assert.AreEqual(30.00m, shares[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_ExcludedAttendee_ApportionsZero()
    {
        // Arrange
        var splitter = new Splitter();
        var bill = new Bill(90.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice"), new("Bob"), new("Cara", 1, false) };

        // Act
        List<Share> shares = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        // Assert
        Assert.AreEqual(45.00m, shares[0].Amount);
        Assert.AreEqual(45.00m, shares[1].Amount);
        Assert.AreEqual(0.00m, shares[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndPercentTip_ApportionsTotal()
    {
        // Arrange
        var splitter = new Splitter();
        var bill = new Bill(100.00m, 13.00m, TipMode.Percent, 15m);
        var attendees = new List<Attendee> { new("Alice"), new("Bob") };

        // Act
        List<Share> shares = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(128.00m, bill.GrandTotal);
        Assert.AreEqual(128.00m, shares.Sum(s => s.Amount));
        Assert.AreEqual(64.00m, shares[0].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndFixedTip_ApportionsTotal()
    {
        // Arrange
        var splitter = new Splitter();
        var bill = new Bill(100.00m, 13.00m, TipMode.Fixed, 20.00m);
        var attendees = new List<Attendee> { new("Alice"), new("Bob") };

        // Act
        List<Share> shares = splitter.CalculateShares(bill, attendees, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(133.00m, bill.GrandTotal);
        Assert.AreEqual(133.00m, shares.Sum(s => s.Amount));
        Assert.AreEqual(66.50m, shares[0].Amount);
    }
}