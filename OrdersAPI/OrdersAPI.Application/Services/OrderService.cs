using Microsoft.Extensions.Logging;
using OrdersAPI.Application.DTOs;
using OrdersAPI.Application.Interfaces;
using OrdersAPI.Domain.Entities;
using OrdersAPI.Domain.Interfaces;

namespace OrdersAPI.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Creating order {OrderId} for customer {CustomerName}", 
                    request.OrderId, request.CustomerName);

                // Check if order already exists
                if (await _orderRepository.OrderExistsAsync(request.OrderId))
                {
                    _logger.LogWarning("Order {OrderId} already exists", request.OrderId);
                    throw new InvalidOperationException($"Order with ID {request.OrderId} already exists");
                }

                // Validate items
                if (!request.Items.Any())
                {
                    throw new ArgumentException("Order must contain at least one item");
                }

                // Validate that all ProductIds are unique
                var duplicateProducts = request.Items
                    .GroupBy(i => i.ProductId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateProducts.Any())
                {
                    throw new ArgumentException($"Duplicate products found: {string.Join(", ", duplicateProducts)}");
                }

                // Map DTO to Entity
                var order = new Order
                {
                    OrderId = request.OrderId,
                    CustomerName = request.CustomerName.Trim(),
                    CreatedAt = request.CreatedAt,
                    Items = request.Items.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        OrderId = request.OrderId
                    }).ToList()
                };

                // Create order
                var createdOrder = await _orderRepository.CreateOrderAsync(order);
                
                _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerName} with {ItemCount} items", 
                    createdOrder.OrderId, createdOrder.CustomerName, createdOrder.Items.Count);

                return new CreateOrderResponse
                {
                    OrderId = createdOrder.OrderId,
                    Message = "Order created successfully",
                    CreatedAt = createdOrder.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order {OrderId}", request.OrderId);
                throw;
            }
        }
    }
}