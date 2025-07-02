using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
} 