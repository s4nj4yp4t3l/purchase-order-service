using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PurchaseOrder.Api.Concrete;
using PurchaseOrder.Data;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace PurchaseOrder.Api.UnitTests.Concrete
{
    [ExcludeFromCodeCoverageAttribute]
    public class PurchaseOrderServiceTests
    {
        private readonly Mock<ILogger<PurchaseOrderService>> _mockLogger;
        private readonly Mock<IValidator<PurchaseOrderRequest>> _mockValidator;
        private readonly Mock<IPurchaseOrderRepository> _mockPurchaseOrderRepository;

        public PurchaseOrderServiceTests()
        {
            // Setup the services and classes we need in our calls.
            _mockLogger = new Mock<ILogger<PurchaseOrderService>>();
            _mockValidator = new Mock<IValidator<PurchaseOrderRequest>>();
            _mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
        }

        [Fact]
        public async Task GetPurchaseOrderAsync_Returns_Http200_OkResult_PurchaseOrderResponse()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 5;
            PurchaseOrderResponse? expected = SampleData.PurchaseOrderResponses.Where(x => x.PoId == poId).FirstOrDefault();

            _mockPurchaseOrderRepository
                .Setup(x => x.GetPurchaseOrderAsync(It.IsAny<int>()))
                .Returns(() => Task.FromResult(expected));

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.GetPurchaseOrderAsync(poId);
            PurchaseOrderResponse? resultResponse = (result as Ok<PurchaseOrderResponse>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.PoId.Should().Be(expected!.PoId, $"because PoId of {expected!.PoId} was chosen."); // Property should match.
            resultResponse!.CustomerId.Should().Be(expected!.CustomerId, $"because CustomerId of {expected!.CustomerId} was chosen."); // Property should match.
            resultResponse!.Items.Count.Should().Be(expected!.Items.Count, "because there should the same number of items in the collection."); // Matching collection count.
            resultResponse!.Total.Should().Be(expected!.Total, $"because Total of {expected!.Total} was chosen."); // Property should match.
        }

        [Fact]
        public async Task GetPurchaseOrderAsync_Http404_NotFoundResult_EmptyList()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 5;
            string expected = $"Purchase order with id {poId} not found";
            PurchaseOrderResponse? purchaseOrderResponse = null;

            _mockPurchaseOrderRepository
                .Setup(x => x.GetPurchaseOrderAsync(It.IsAny<int>()))
                .Returns(() => Task.FromResult(purchaseOrderResponse));

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.GetPurchaseOrderAsync(poId);
            string? resultResponse = (result as NotFound<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be a string message."); // Message.
            resultResponse!.Should().Be(expected);
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_Returns_Http201_CreatedResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 5;
            PurchaseOrderResponse? expected = SampleData.PurchaseOrderResponses.Where(x => x.PoId == poId).FirstOrDefault();
            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;

            _mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new ValidationResult())); // Will mean IsValid = true

            _mockPurchaseOrderRepository
                .Setup(x => x.ProcessPurchaseOrderAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()))
                .Returns(() => Task.FromResult(expected));

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest);
            PurchaseOrderResponse? resultResponse = (result as Created<PurchaseOrderResponse>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.PoId.Should().Be(expected!.PoId, $"because PoId of {expected!.PoId} was chosen."); // Property should match.
            resultResponse!.CustomerId.Should().Be(expected!.CustomerId, $"because CustomerId of {expected!.CustomerId} was chosen."); // Property should match.
            resultResponse!.Items.Count.Should().Be(expected!.Items.Count, "because there should the same number of items in the collection."); // Matching collection count.
            resultResponse!.Total.Should().Be(expected!.Total, $"because Total of {expected!.Total} was chosen."); // Property should match.
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_InvalidCustomerId_Returns_Http400_ValidationProblemResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;
            ValidationResult validationResult = new()
            {
                Errors = [ new ValidationFailure() {
                    Severity = Severity.Error,
                    PropertyName = "CustomerId",
                    ErrorMessage = "'CustomerId' must be greater than '0'."} ]
            };

            ProblemDetails expected = new()
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "One or more validation errors occurred.",
            };

            _mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(validationResult)); // Will mean IsValid = false

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest);
            ProblemHttpResult? resultResponse = result as ProblemHttpResult;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest, "because this is a bad request."); // Bad request status code.
            resultResponse!.ProblemDetails.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.ProblemDetails.Title.Should().Be(expected.Title, $"because Title of {expected.Title} was chosen."); // Property should match.
            resultResponse!.ProblemDetails.Status.Should().Be(expected.Status, $"because Status of {expected.Status} was chosen."); // Property should match.
            // Can't see "problemHttpResultResponse!.ProblemDetails.Errors" because of this issue:
            // https://github.com/dotnet/aspnetcore/issues/41634;
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_EmptyItems_Returns_Http400_ValidationProblemResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            ValidationResult validationResult = new()
            {
                Errors = [ new ValidationFailure() {
                Severity = Severity.Error,
                PropertyName = "Items.Count",
                ErrorMessage = "'Items Count' must be greater than '0'."} ]
            };

            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;
            ProblemDetails expected = new()
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "One or more validation errors occurred.",
            };

            _mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(validationResult)); // Will mean IsValid = false

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest);
            ProblemHttpResult? resultResponse = result as ProblemHttpResult;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest, "because this is a bad request."); // Bad request status code.
            resultResponse!.ProblemDetails.Should().NotBeNull("because there should be an object returned."); // 1 object.
            resultResponse!.ProblemDetails.Title.Should().Be(expected.Title, $"because Title of {expected.Title} was chosen."); // Property should match.
            resultResponse!.ProblemDetails.Status.Should().Be(expected.Status, $"because Status of {expected.Status} was chosen."); // Property should match.
            // Can't see "problemHttpResultResponse!.ProblemDetails.Errors" because of this issue:
            // https://github.com/dotnet/aspnetcore/issues/41634;
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_PurchaseOrderIsNull_Returns_Http422_UnprocessableEntityResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            PurchaseOrderResponse? purchaseOrderResponse = null;
            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;

            string expected = "Purchase order could not be processed";

            _mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new ValidationResult())); // Will mean IsValid = true

            _mockPurchaseOrderRepository
                .Setup(x => x.ProcessPurchaseOrderAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()))
                .Returns(() => Task.FromResult(purchaseOrderResponse));

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest);
            string? resultResponse = (result as UnprocessableEntity<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be a string message."); // Message.
            resultResponse!.Should().Be(expected);
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_ExceptionThrown_Returns_Http422_UnprocessableEntityResult()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;

            string expected = "Exception occurred";

            _mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new ValidationResult())); // Will mean IsValid = true

            _mockPurchaseOrderRepository
                .Setup(x => x.ProcessPurchaseOrderAsync(It.IsAny<PurchaseOrderRequest>(), It.IsAny<List<string>>(), It.IsAny<List<int>>()))
                .Throws(new Exception());

            // Don't need to setup any other mocks as we exit the sut before they are hit.

            PurchaseOrderService purchaseOrderService = new(
                _mockLogger.Object,
                _mockValidator.Object,
                _mockPurchaseOrderRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await purchaseOrderService.ProcessPurchaseOrderAsync(purchaseOrderRequest);
            string? resultResponse = (result as UnprocessableEntity<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be a string message."); // Message.
            resultResponse!.Should().Be(expected);
        }
    }
}