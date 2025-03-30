using FluentValidation.Results;
using FluentValidation;
using FluentValidation.TestHelper;
using PurchaseOrder.Api.Validations;
using PurchaseOrder.Models.Concrete;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using PurchaseOrder.Data;

namespace PurchaseOrder.Api.UnitTests.Validations
{
    [ExcludeFromCodeCoverageAttribute]
    public class PurchaseOrderRequestValidatorTests
    {
        [Fact]
        public void PurchaseOrderRequestValidator_InvalidCustomerId_Returns_TestValidationResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            List<Item> items = [.. SampleData.Items.Where(x => x.Id == 1)];
            ValidationResult expected = new()
            {
                Errors = [ new ValidationFailure() {
                    Severity = Severity.Error,
                    PropertyName = "CustomerId",
                    ErrorCode = "GreaterThanValidator",
                    ErrorMessage = "'Customer Id' must be greater than '0'."} ]
            };

            PurchaseOrderRequestValidator purchaseOrderRequestValidation = new();

            var purchaseOrderRequest = new PurchaseOrderRequest() { CustomerId = 0, Items = items };

            // ACT - invokes the method under test.
            TestValidationResult<PurchaseOrderRequest> result = purchaseOrderRequestValidation.TestValidate(purchaseOrderRequest);

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
            result.Errors.Count.Should().Be(expected.Errors.Count, "because there should the same number of items in the collection."); // Matching collection count.
            result.IsValid.Should().Be(false, $"because IsValid of false was chosen."); // Property should match.
            result.Errors.First().Severity.Should().Be(expected.Errors.First().Severity, $"because Severity of {expected.Errors.First().Severity} was chosen."); // Property should match.
            result.Errors.First().ErrorCode.Should().Be(expected.Errors.First().ErrorCode, $"because ErrorCode of {expected.Errors.First().ErrorCode} was chosen."); // Property should match.
            result.Errors.First().ErrorMessage.Should().Be(expected.Errors.First().ErrorMessage, $"because ErrorMessage of {expected.Errors.First().ErrorMessage} was chosen."); // Property should match.
            result.Errors.First().PropertyName.Should().Be(expected.Errors.First().PropertyName, $"because PropertyName of {expected.Errors.First().PropertyName} was chosen."); // Property should match.
        }

        [Fact]
        public void PurchaseOrderRequestValidator_EmptyItems_Returns_TestValidationResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            List<Item> items = [ ];
            ValidationResult expected = new()
            {
                Errors = [ new ValidationFailure() {
                    Severity = Severity.Error,
                    PropertyName = "Items.Count",
                    ErrorCode = "GreaterThanValidator",
                    ErrorMessage = "'Items Count' must be greater than '0'."} ]
            };

            PurchaseOrderRequestValidator purchaseOrderRequestValidation = new();

            var purchaseOrderRequest = new PurchaseOrderRequest() { CustomerId = 1, Items = items };

            // ACT - invokes the method under test.
            TestValidationResult<PurchaseOrderRequest> result = purchaseOrderRequestValidation.TestValidate(purchaseOrderRequest);

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result.ShouldHaveValidationErrorFor(x => x.Items.Count);
            result.Errors.Count.Should().Be(expected.Errors.Count, "because there should the same number of items in the collection."); // Matching collection count.
            result.IsValid.Should().Be(false, $"because IsValid of false was chosen."); // Property should match.
            result.Errors.First().Severity.Should().Be(expected.Errors.First().Severity, $"because Severity of {expected.Errors.First().Severity} was chosen."); // Property should match.
            result.Errors.First().ErrorCode.Should().Be(expected.Errors.First().ErrorCode, $"because ErrorCode of {expected.Errors.First().ErrorCode} was chosen."); // Property should match.
            result.Errors.First().ErrorMessage.Should().Be(expected.Errors.First().ErrorMessage, $"because ErrorMessage of {expected.Errors.First().ErrorMessage} was chosen."); // Property should match.
            result.Errors.First().PropertyName.Should().Be(expected.Errors.First().PropertyName, $"because PropertyName of {expected.Errors.First().PropertyName} was chosen."); // Property should match.
        }
    }
}