using MarketingScheduler.Common;
using MarketingScheduler.Models;
using MarketingScheduler.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MarketingScheduler.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _customerService;
        private readonly Mock<IMongoCollection<Customer>> _mockCollection;
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IOptions<DatabaseSettings>> _mockOptions;

        public CustomerServiceTests()
        {
            _mockCollection = new Mock<IMongoCollection<Customer>>();
            _mockClient = new Mock<IMongoClient>();
            _mockOptions = new Mock<IOptions<DatabaseSettings>>();

            _mockOptions.Setup(options => options.Value)
                .Returns(new DatabaseSettings
                {
                    DatabaseName = "testDatabase",
                    CollectionName = "testCollection"
                });

            _mockClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null)).Returns(Mock.Of<IMongoDatabase>());
            _mockClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null).GetCollection<Customer>(It.IsAny<string>(), null)).Returns(_mockCollection.Object);

            _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Customer>>(), It.IsAny<FindOptions<Customer>>(), default))
                .ReturnsAsync(Mock.Of<IAsyncCursor<Customer>>());

            _mockCollection.Setup(x => x.InsertManyAsync(It.IsAny<IEnumerable<Customer>>(), It.IsAny<InsertManyOptions>(), default))
                .Returns(Task.CompletedTask);

            // _mockCollection.Setup(x => x.UpdateOneAsync(It.IsAny<FilterDefinition<Customer>>(), It.IsAny<UpdateDefinition<Customer>>(), It.IsAny<UpdateOptions>(), default))
            //     .ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

            _customerService = new CustomerService(_mockOptions.Object, _mockClient.Object)
            {
                _customers = _mockCollection.Object
            };
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsList()
        {
            // Act
            var actualCustomers = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.Empty(actualCustomers);
            Assert.IsType<List<Customer>>(actualCustomers);
        }

        [Fact]
        public async Task AddCustomersAsync_AddsNewCustomers_ReturnsTrue()
        {
            // Arrange
            var newCustomers = new List<Customer>
            {
                new Customer (Guid.NewGuid(), "Customer A", Frequency.Daily, null ),
                new Customer (Guid.NewGuid(), "Customer B", Frequency.Never, null)
            };

            // Act
            var result = await _customerService.AddCustomersAsync(newCustomers);

            // Assert
            Assert.True(result);
        }
    }
}

