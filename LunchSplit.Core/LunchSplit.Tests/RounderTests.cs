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



    [TestMethod]
    public void RoundShares_BankersRounding_RoundsToNearestEven()
    {
        // Arrange
        var rounder = new Rounder();
        var raw = new List<Share> { new("Alice", 10.325m), new("Bob", 10.335m) };

        // Act
        List<Share> result = rounder.RoundShares(raw, RoundingMode.Bankers);

        // Assert
        Assert.AreEqual(10.32m, result[0].Amount);
        Assert.AreEqual(10.34m, result[1].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundUp_AppliesCeilingToCents()
    {
        // Arrange
        var rounder = new Rounder();
        var raw = new List<Share> { new("Alice", 10.331m) };

        // Act
        List<Share> result = rounder.RoundShares(raw, RoundingMode.RoundUp);

        // Assert
        Assert.AreEqual(10.34m, result[0].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundDown_AppliesFloorToCents()
    {
        // Arrange
        var rounder = new Rounder();
        var raw = new List<Share> { new("Alice", 10.339m) };

        // Act
        List<Share> result = rounder.RoundShares(raw, RoundingMode.RoundDown);

        // Assert
        Assert.AreEqual(10.33m, result[0].Amount);
    }
}