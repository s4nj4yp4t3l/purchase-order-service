using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using PurchaseOrder.Api.Concrete;
using System.Diagnostics.CodeAnalysis;

namespace PurchaseOrder.Api.UnitTests.Concrete
{
    [ExcludeFromCodeCoverageAttribute]
    public class HealthServiceTests
    {
        private readonly Mock<ILogger<HealthService>> _mockLogger;

        public HealthServiceTests()
        {
            // Setup the services and classes we need in our calls.
            _mockLogger = new Mock<ILogger<HealthService>>();
        }

        [Fact]
        public async Task GetHealthStatusAsync_Returns_Status()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            string expected = $"Healthy @";

            HealthService healthService = new(_mockLogger.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await healthService.GetHealthStatusAsync();
            string? resultResponse = (result as Ok<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result.Should().NotBeNull("because there should be a string returned."); // String returned.
            resultResponse.Should().BeOfType<string>("because there should be a string returned."); // String returned.
            resultResponse.Should().StartWith(expected); // result = expected.
        }
    }
}