using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 