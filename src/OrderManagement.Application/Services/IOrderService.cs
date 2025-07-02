using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
} 