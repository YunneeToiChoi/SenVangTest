using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task<bool> ExistsAsync(int id);
} 