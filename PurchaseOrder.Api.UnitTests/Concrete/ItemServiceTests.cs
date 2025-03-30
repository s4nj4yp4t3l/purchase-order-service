using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using PurchaseOrder.Api.Concrete;
using PurchaseOrder.Models.Concrete;
using PurchaseOrder.Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;
using PurchaseOrder.Data;

namespace PurchaseOrder.Api.UnitTests.Concrete
{
    [ExcludeFromCodeCoverageAttribute]
    public class ItemServiceTests
    {
        private readonly Mock<ILogger<ItemService>> _mockLogger;
        private readonly Mock<IItemRepository> _mockItemRepository;

        public ItemServiceTests()
        {
            // Setup the services and classes we need in our calls.
            _mockLogger = new Mock<ILogger<ItemService>>();
            _mockItemRepository = new Mock<IItemRepository>();
        }

        [Fact]
        public async Task GetItemsAsync_Returns_Http200_OkResult_ListOfItems()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            List<Item> expected = SampleData.Items;

            _mockItemRepository
                .Setup(x => x.GetItemsAsync())
                .Returns(() => Task.FromResult(expected));

            ItemService itemService = new(_mockLogger.Object, _mockItemRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await itemService.GetItemsAsync();
            List<Item>? resultResponse = (result as Ok<List<Item>>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be multiple items in the collection."); // Multiple items.
            resultResponse!.Count.Should().BeGreaterThan(0, "because there should be at least one item in the collection."); // At least one item.
            resultResponse!.First().Id.Should().Be(expected.First().Id, $"because the first item has an Id of {expected.First().Id}."); // Property should match.
            resultResponse!.First().Title.Should().Be(expected.First().Title, $"because the first item has an Title of {expected.First().Title}."); // Property should match.
            resultResponse!.First().Price.Should().Be(expected.First().Price, $"because the first item has an Price of {expected.First().Price}."); // Property should match.
            resultResponse!.First().IsPhysicalItem.Should().Be(expected.First().IsPhysicalItem, $"because the first item has an IsPhysicalItem of {expected.First().IsPhysicalItem}."); // Property should match.
            resultResponse!.Should().OnlyHaveUniqueItems("because the table is normalized."); // Normalized table.
            resultResponse!.Should().BeInAscendingOrder(x => x.Id, "because Id is the primary key"); // Id is primary key.
        }

        [Fact]
        public async Task GetItemsAsync_Returns_Http404_NotFoundResult_EmptyList()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            string expected = "No items found";
            List<Item> items = [];

            _mockItemRepository
                .Setup(x => x.GetItemsAsync())
                .Returns(() => Task.FromResult(items));

            ItemService itemService = new(_mockLogger.Object, _mockItemRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await itemService.GetItemsAsync();
            string? resultResponse = (result as NotFound<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be a string message."); // Message.
            resultResponse!.Should().Be(expected);
        }
    }
}