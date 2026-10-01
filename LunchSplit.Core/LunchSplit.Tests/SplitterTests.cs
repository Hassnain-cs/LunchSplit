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
}