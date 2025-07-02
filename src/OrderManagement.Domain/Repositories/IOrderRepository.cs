using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByIdWithItemsAsync(int id);
    Task<Order> AddAsync(Order order);
} 