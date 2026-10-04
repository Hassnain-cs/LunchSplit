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

    [TestMethod]
    public void Validate_NegativeSubtotal_ReturnsFail()
    {
        // Arrange
        var validator = new BillValidator();
        var bill = new Bill(-10.00m, 0m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice") };

        // Act
        ValidationResult result = validator.Validate(bill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Subtotal")));
    }

    [TestMethod]
    public void Validate_NegativeTax_ReturnsFail()
    {
        // Arrange
        var validator = new BillValidator();
        var bill = new Bill(100.00m, -1.00m, TipMode.None, 0m);
        var attendees = new List<Attendee> { new("Alice") };

        // Act
        ValidationResult result = validator.Validate(bill, attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("Tax")));
    }

    [TestMethod]
    public void Validate_ZeroAttendees_ReturnsFail()
    {
        // Arrange
        var validator = new BillValidator();
        List<Attendee> attendees = null!;

        // Act
        ValidationResult result = validator.Validate(ValidBill(), attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_NegativeAttendeeWeight_ReturnsFail()
    {
        // Arrange
        var validator = new BillValidator();
        var attendees = new List<Attendee> { new("Alice", -1) };

        // Act
        ValidationResult result = validator.Validate(ValidBill(), attendees);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("weight")));
    }

      
}