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
}