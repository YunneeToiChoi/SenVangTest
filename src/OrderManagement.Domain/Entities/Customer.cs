using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Domain.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [StringLength(15)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Navigation property
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
} 