using AutoFixture;
using OrdersAPI.Application.DTOs;
using OrdersAPI.Domain.Entities;

namespace OrdersAPI.UnitTests.TestHelpers
{
    public static class TestDataBuilder
    {
        private static readonly Fixture _fixture = new();

        public static CreateOrderRequest CreateValidOrderRequest()
        {
            return new CreateOrderRequest
            {
                OrderId = Guid.NewGuid(),
                CustomerName = GenerateSafeCustomerName(),
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItemDto>
                {
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = _fixture.Create<int>() % 10 + 1 // 1-10
                    },
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = _fixture.Create<int>() % 5 + 1 // 1-5
                    }
                }
            };
        }

        public static CreateOrderRequest CreateOrderRequestWithEmptyItems()
        {
            var request = CreateValidOrderRequest();
            request.Items = new List<OrderItemDto>();
            return request;
        }

        public static CreateOrderRequest CreateOrderRequestWithDuplicateProducts()
        {
            var productId = Guid.NewGuid();
            return new CreateOrderRequest
            {
                OrderId = Guid.NewGuid(),
                CustomerName = GenerateSafeCustomerName(),
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = productId, Quantity = 1 },
                    new() { ProductId = productId, Quantity = 2 } // Duplicate
                }
            };
        }

        public static Order CreateValidOrder()
        {
            var orderId = Guid.NewGuid();
            return new Order
            {
                OrderId = orderId,
                CustomerName = GenerateSafeCustomerName(),
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new()
                    {
                        Id = 1,
                        ProductId = Guid.NewGuid(),
                        Quantity = 2,
                        OrderId = orderId
                    }
                }
            };
        }
        
        private static string GenerateSafeCustomerName()
        {
            var name = _fixture.Create<string>();
            
            if (string.IsNullOrEmpty(name))
                name = "Test Customer";
            
            return name.Length > 50 ? name.Substring(0, 50) : name;
        }
    }
}