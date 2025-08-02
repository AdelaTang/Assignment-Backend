using Microsoft.Extensions.DependencyInjection;
using OrdersAPI.Application.Interfaces;
using OrdersAPI.Application.Services;

namespace OrdersAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            
            return services;
        }
    }
}