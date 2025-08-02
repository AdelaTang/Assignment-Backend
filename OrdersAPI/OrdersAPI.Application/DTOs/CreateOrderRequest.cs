using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Application.DTOs
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "OrderId is required")]
        public Guid OrderId { get; set; }
        
        [Required(ErrorMessage = "CustomerName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "CustomerName must be between 1 and 100 characters")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemDto> Items { get; set; } = new();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}