using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public OrderRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    // Alias methods for backward compatibility
    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await GetByDateRangeAsync(fromDate, toDate);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        return await GetByCustomerIdAsync(customerId);
    }
} 