using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersAPI.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        
        public virtual Order Order { get; set; } = null!;
    }
}