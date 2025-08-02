using OrdersAPI.Application.DTOs;

namespace OrdersAPI.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    }
}