using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
    
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
} 