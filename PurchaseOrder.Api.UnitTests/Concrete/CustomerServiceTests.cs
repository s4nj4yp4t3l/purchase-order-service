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
    public class CustomerServiceTests
    {
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;

        public CustomerServiceTests()
        {
            // Setup the services and classes we need in our calls.
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task GetCustomersAsync_Returns_Http200_OkResult_ListOfCustomers()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            List<Customer> expected = SampleData.Customers;

            _mockCustomerRepository
                .Setup(x => x.GetCustomersAsync())
                .Returns(() => Task.FromResult(expected));

            CustomerService customerService = new(_mockLogger.Object, _mockCustomerRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await customerService.GetCustomersAsync();
            List<Customer>? resultResponse = (result as Ok<List<Customer>>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be multiple customers in the collection."); // Multiple customers.
            resultResponse!.Count.Should().BeGreaterThan(0, "because there should be at least one customer in the collection."); // At least one customer.
            resultResponse!.First().Id.Should().Be(expected.First().Id, $"because the first customer has an Id of {expected.First().Id}."); // Property should match.
            resultResponse!.First().AddressId.Should().Be(expected.First().AddressId, $"because the first customer has an AddressId of {expected.First().AddressId}."); // Property should match.
            resultResponse!.First().MembershipTypes.Count.Should().Be(expected.First().MembershipTypes.Count, "because there should the same number of items in the collection."); // Matching collection count.
            resultResponse!.First().Forename.Should().Be(expected.First().Forename, $"because the first customer has an Forename of {expected.First().Forename}."); // Property should match.
            resultResponse!.First().Surname.Should().Be(expected.First().Surname, $"because the first customer has an Surname of {expected.First().Surname}."); // Property should match.
            resultResponse!.Should().OnlyHaveUniqueItems("because the table is normalized."); // Normalized table.
            resultResponse!.Should().BeInAscendingOrder(x => x.Id, "because Id is the primary key"); // Id is primary key.
        }

        [Fact]
        public async Task GetCustomersAsync_Returns_Http404_NotFoundResult_EmptyList()
        {
            // ARRANGE - initializes objects and sets the value of the data that is passed to the method tested.
            string expected = "No customers found";
            List<Customer> customers = [];

            _mockCustomerRepository
                .Setup(x => x.GetCustomersAsync())
                .Returns(() => Task.FromResult(customers));

            CustomerService customerService = new(_mockLogger.Object, _mockCustomerRepository.Object); // sut.

            // ACT - invokes the method under test.
            IResult result = await customerService.GetCustomersAsync();
            string? resultResponse = (result as NotFound<string>)!.Value;

            // ASSERT - checks that the tested method behaves as expected.
            // Using Fluent Assertions.
            resultResponse!.Should().NotBeNull("because there should be a string message."); // Message.
            resultResponse!.Should().Be(expected);
        }
    }
}
