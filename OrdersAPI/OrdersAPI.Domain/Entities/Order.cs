using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Domain.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}