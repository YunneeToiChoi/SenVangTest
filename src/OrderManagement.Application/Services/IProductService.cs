using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
} 