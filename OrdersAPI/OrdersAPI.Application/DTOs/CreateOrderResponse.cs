namespace OrdersAPI.Application.DTOs
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}