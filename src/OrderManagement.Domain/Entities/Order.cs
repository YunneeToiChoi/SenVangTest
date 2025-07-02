using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }
    
    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 