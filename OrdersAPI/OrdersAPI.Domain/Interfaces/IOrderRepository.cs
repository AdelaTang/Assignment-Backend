using OrdersAPI.Domain.Entities;

namespace OrdersAPI.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<bool> OrderExistsAsync(Guid orderId);
    }
}