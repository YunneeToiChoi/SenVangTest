using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderManagementDbContext _context;

    public ProductRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int productId)
    {
        return await _context.Products.FindAsync(productId);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<bool> ExistsAsync(int productId)
    {
        return await _context.Products.AnyAsync(p => p.ProductId == productId);
    }

    public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> productIds)
    {
        return await _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToListAsync();
    }
} 