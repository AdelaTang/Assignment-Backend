using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersAPI.Application.Services;
using OrdersAPI.Domain.Entities;
using OrdersAPI.Domain.Interfaces;
using OrdersAPI.UnitTests.TestHelpers;
using Xunit;

namespace OrdersAPI.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _service = new OrderService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var request = TestDataBuilder.CreateValidOrderRequest();
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(false);
            
            _mockRepository.Setup(x => x.CreateOrderAsync(It.IsAny<Order>()))
                          .ReturnsAsync((Order order) => order);

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(request.OrderId);
            result.Message.Should().Be("Order created successfully");
            result.CreatedAt.Should().Be(request.CreatedAt);
            
            _mockRepository.Verify(x => x.OrderExistsAsync(request.OrderId), Times.Once);
            _mockRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_DuplicateOrderId_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = TestDataBuilder.CreateValidOrderRequest();
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateOrderAsync(request));
            
            exception.Message.Should().Contain("already exists");
            
            _mockRepository.Verify(x => x.OrderExistsAsync(request.OrderId), Times.Once);
            _mockRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptyItems_ThrowsArgumentException()
        {
            // Arrange
            var request = TestDataBuilder.CreateOrderRequestWithEmptyItems();
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(request));
            
            exception.Message.Should().Contain("at least one item");
            
            _mockRepository.Verify(x => x.OrderExistsAsync(request.OrderId), Times.Once);
            _mockRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_DuplicateProducts_ThrowsArgumentException()
        {
            // Arrange
            var request = TestDataBuilder.CreateOrderRequestWithDuplicateProducts();
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateOrderAsync(request));
            
            exception.Message.Should().Contain("Duplicate products found");
            
            _mockRepository.Verify(x => x.OrderExistsAsync(request.OrderId), Times.Once);
            _mockRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrderAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var request = TestDataBuilder.CreateValidOrderRequest();
            var expectedException = new InvalidOperationException("Database error");
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(false);
            
            _mockRepository.Setup(x => x.CreateOrderAsync(It.IsAny<Order>()))
                          .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateOrderAsync(request));
            
            exception.Should().Be(expectedException);
            
            _mockRepository.Verify(x => x.OrderExistsAsync(request.OrderId), Times.Once);
            _mockRepository.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidRequest_CreatesOrderWithCorrectMapping()
        {
            // Arrange
            var request = TestDataBuilder.CreateValidOrderRequest();
            Order? capturedOrder = null;
            
            _mockRepository.Setup(x => x.OrderExistsAsync(request.OrderId))
                          .ReturnsAsync(false);
            
            _mockRepository.Setup(x => x.CreateOrderAsync(It.IsAny<Order>()))
                          .Callback<Order>(order => capturedOrder = order)
                          .ReturnsAsync((Order order) => order);

            // Act
            await _service.CreateOrderAsync(request);

            // Assert
            capturedOrder.Should().NotBeNull();
            capturedOrder!.OrderId.Should().Be(request.OrderId);
            capturedOrder.CustomerName.Should().Be(request.CustomerName.Trim());
            capturedOrder.CreatedAt.Should().Be(request.CreatedAt);
            capturedOrder.Items.Should().HaveCount(request.Items.Count);
            
            foreach (var requestItem in request.Items)
            {
                var orderItem = capturedOrder.Items.FirstOrDefault(i => i.ProductId == requestItem.ProductId);
                orderItem.Should().NotBeNull();
                orderItem!.Quantity.Should().Be(requestItem.Quantity);
                orderItem.OrderId.Should().Be(request.OrderId);
            }
        }
    }
}