using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PurchaseOrder.Data;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Repository.Concrete;
using System.Diagnostics.CodeAnalysis;

namespace PurchaseOrder.Repository.UnitTests.Concrete
{
    [ExcludeFromCodeCoverageAttribute]
    public class PurchaseOrderRepositoryTests
    {
        private readonly Mock<ILogger<PurchaseOrderRepository>> _mockLogger;

        public PurchaseOrderRepositoryTests()
        {
            // Setup the services and classes we need in our calls.
            _mockLogger = new Mock<ILogger<PurchaseOrderRepository>>();
        }

        [Fact]
        public async Task GetPurchaseOrderAsync_Returns_PurchaseOrderResponse()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 1;
            PurchaseOrderResponse expected = SampleData.PurchaseOrderResponses.Where(x => x.PoId == poId).FirstOrDefault()!;

            PurchaseOrderRepository purchaseOrderRepository = new(_mockLogger.Object);

            // ACT - invokes the method under test.
            PurchaseOrderResponse? result = await purchaseOrderRepository.GetPurchaseOrderAsync(poId);

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result!.Should().NotBeNull("because there should be an object returned."); // Multiple items.
            result!.PoId.Should().Be(expected.PoId, $"because the first item was has an PoId of {expected.PoId}."); // Property should match.
            result!.CustomerId.Should().Be(expected.CustomerId, $"because the first item was has an CustomerId of {expected.CustomerId}."); // Property should match.
            result!.Items.Count.Should().Be(expected.Items.Count, "because there should the same number of items in the collection."); // Matching collection count.
            result!.Total.Should().Be(expected.Total, $"because the first item has an Forename of {expected.Total}."); // Property should match.
        }

        [Fact]
        public async Task GetPurchaseOrderAsync_Returns_NullPurchaseOrder()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 999;
            PurchaseOrderResponse? expected = null;

            PurchaseOrderRepository purchaseOrderRepository = new(_mockLogger.Object);

            // ACT - invokes the method under test.
            PurchaseOrderResponse? result = await purchaseOrderRepository.GetPurchaseOrderAsync(poId);

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result!.Should().BeNull("because there should be no object returned."); // Null object.
            result!.Should().Be(expected);
        }

        [Fact]
        public async Task ProcessPurchaseOrderAsync_Returns_PurchaseOrderResponse()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            int poId = 5;
            PurchaseOrderResponse expected = SampleData.PurchaseOrderResponses.Where(x => x.PoId == poId).FirstOrDefault()!;
            PurchaseOrderRequest purchaseOrderRequest = SampleData.PurchaseOrderRequest;
            List<string> membershipTypes = [.. SampleData.Items.Where(x => x.Title == Models.Constants.Membership.BOOK_CLUB_MEMBERSHIP).Select(y => y.Title)];
            List<int> physicalItemIds = [.. purchaseOrderRequest.Items.Where(x => x.IsPhysicalItem == true).Select(y => y.Id)];

            PurchaseOrderRepository purchaseOrderRepository = new(_mockLogger.Object);

            // ACT - invokes the method under test.
            PurchaseOrderResponse? result = await purchaseOrderRepository.ProcessPurchaseOrderAsync(
                purchaseOrderRequest, membershipTypes, physicalItemIds);

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            result!.Should().NotBeNull("because there should be an object returned."); // Multiple items.
            result!.PoId.Should().Be(expected.PoId, $"because the first item was has an PoId of {expected.PoId}."); // Property should match.
            result!.CustomerId.Should().Be(expected.CustomerId, $"because the first item was has an CustomerId of {expected.CustomerId}."); // Property should match.
            result!.Items.Count.Should().Be(expected.Items.Count, "because there should the same number of items in the collection."); // Matching collection count.
            result!.Total.Should().Be(expected.Total, $"because the first item has an Forename of {expected.Total}."); // Property should match.
        }
    }
}