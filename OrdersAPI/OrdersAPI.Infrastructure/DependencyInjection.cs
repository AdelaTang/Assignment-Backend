using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersAPI.Domain.Interfaces;
using OrdersAPI.Infrastructure.Data;
using OrdersAPI.Infrastructure.Repositories;

namespace OrdersAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(OrdersDbContext).Assembly.FullName)));

            // Add repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            return services;
        }
    }
}