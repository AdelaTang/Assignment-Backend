using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrdersAPI.Domain.Entities;
using OrdersAPI.Domain.Interfaces;
using OrdersAPI.Infrastructure.Data;

namespace OrdersAPI.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(OrdersDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                _logger.LogInformation("Order {OrderId} created successfully in database", order.OrderId);
                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order {OrderId} in database", order.OrderId);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId} from database", orderId);
                throw;
            }
        }

        public async Task<bool> OrderExistsAsync(Guid orderId)
        {
            try
            {
                return await _context.Orders
                    .AnyAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if order {OrderId} exists in database", orderId);
                throw;
            }
        }
    }
}