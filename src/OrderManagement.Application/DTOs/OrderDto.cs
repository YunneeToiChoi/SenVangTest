using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Application.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderDto
{
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public class CreateOrderItemDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
} 