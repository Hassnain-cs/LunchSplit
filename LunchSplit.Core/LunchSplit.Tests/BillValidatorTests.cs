using LunchSplit.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class BillValidatorTests
{
    private static Bill ValidBill() => new(100.00m, 13.00m, TipMode.Percent, 15m);

    [TestMethod]
    public void Validate_CompleteBillDetails_ReturnsOk()
    {
        // Arrange
        var validator = new BillValidator();
        var attendees = new List<Attendee> { new("Alice") };

        // Act
        ValidationResult result = validator.Validate(ValidBill(), attendees);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_EmptyAttendeeCollection_ReturnsFail()
    {
        // Arrange
        var validator = new BillValidator();
        var attendees = new List<Attendee>();

        // Act
        ValidationResult result = validator.Validate(ValidBill(), attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("attendee")));
    }
}