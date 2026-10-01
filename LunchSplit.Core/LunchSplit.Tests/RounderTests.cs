using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Metrics;

namespace LunchSplit.Tests;

[TestClass]
public class RounderTests
{


    [TestMethod]
    public void RoundShares_NoRounding_PreservesRawDecimals()
    {
        // Arrange
        var rounder = new Rounder();
        var raw = new List<Share> { new("Alice", 3.3333m) };

        // Act
        List<Share> result = rounder.RoundShares(raw, RoundingMode.None);

        // Assert
        Assert.AreEqual(3.3333m, result[0].Amount);
    }

}