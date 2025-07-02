using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs;

public class CustomerDto
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
}

public class CreateCustomerDto
{
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
}

public class UpdateCustomerDto
{
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
} 