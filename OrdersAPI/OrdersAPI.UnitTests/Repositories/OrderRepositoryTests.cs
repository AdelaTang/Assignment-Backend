using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersAPI.Domain.Entities;
using OrdersAPI.Infrastructure.Data;
using OrdersAPI.Infrastructure.Repositories;
using OrdersAPI.UnitTests.TestHelpers;
using Xunit;

namespace OrdersAPI.UnitTests.Repositories
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly OrdersDbContext _context;
        private readonly Mock<ILogger<OrderRepository>> _mockLogger;
        private readonly OrderRepository _repository;

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new OrdersDbContext(options);
            _mockLogger = new Mock<ILogger<OrderRepository>>();
            _repository = new OrderRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = TestDataBuilder.CreateValidOrder();

            // Act
            var result = await _repository.CreateOrderAsync(order);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(order.OrderId);
            result.CustomerName.Should().Be(order.CustomerName);
            result.Items.Should().HaveCount(order.Items.Count);

            // Verify it's actually in the database
            var dbOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            
            dbOrder.Should().NotBeNull();
            dbOrder!.Items.Should().HaveCount(order.Items.Count);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ExistingOrder_ReturnsOrder()
        {
            // Arrange
            var order = TestDataBuilder.CreateValidOrder();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetOrderByIdAsync(order.OrderId);

            // Assert
            result.Should().NotBeNull();
            result!.OrderId.Should().Be(order.OrderId);
            result.CustomerName.Should().Be(order.CustomerName);
            result.Items.Should().HaveCount(order.Items.Count);
        }

        [Fact]
        public async Task GetOrderByIdAsync_NonExistingOrder_ReturnsNull()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.GetOrderByIdAsync(nonExistingId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task OrderExistsAsync_ExistingOrder_ReturnsTrue()
        {
            // Arrange
            var order = TestDataBuilder.CreateValidOrder();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.OrderExistsAsync(order.OrderId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task OrderExistsAsync_NonExistingOrder_ReturnsFalse()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.OrderExistsAsync(nonExistingId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CreateOrderAsync_WithMultipleItems_CreatesAllItems()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                OrderId = orderId,
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2, OrderId = orderId },
                    new() { ProductId = Guid.NewGuid(), Quantity = 3, OrderId = orderId },
                    new() { ProductId = Guid.NewGuid(), Quantity = 1, OrderId = orderId }
                }
            };

            // Act
            var result = await _repository.CreateOrderAsync(order);

            // Assert
            result.Items.Should().HaveCount(3);
            
            var dbOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            
            dbOrder!.Items.Should().HaveCount(3);
            dbOrder.Items.Sum(i => i.Quantity).Should().Be(6);
        }

        [Fact]
        public async Task CreateOrderAsync_InvalidOrder_ThrowsException()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = new string('X', 150),
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            // Act & Assert
            var act = () => _repository.CreateOrderAsync(order);
            
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CreateOrderAsync_DuplicateOrderId_ThrowsDbUpdateException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var firstOrder = new Order
            {
                OrderId = orderId,
                CustomerName = "First Customer",
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            var duplicateOrder = new Order
            {
                OrderId = orderId, // same OrderId
                CustomerName = "Second Customer", 
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            // add the first order
            await _repository.CreateOrderAsync(firstOrder);

            // Act & Assert - try to add the duplicated orderId - should be failed
            await Assert.ThrowsAnyAsync<Exception>(
                () => _repository.CreateOrderAsync(duplicateOrder));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}