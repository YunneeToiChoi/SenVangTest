using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private const string PRODUCTS_CACHE_KEY = "products:all";
    private const string PRODUCT_CACHE_KEY = "product:";

    public ProductService(IProductRepository productRepository, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(PRODUCTS_CACHE_KEY);
        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetAllAsync();
        var productDtos = products.Select(MapToDto).ToList();

        await _cacheService.SetAsync(PRODUCTS_CACHE_KEY, productDtos, TimeSpan.FromMinutes(10));
        return productDtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"{PRODUCT_CACHE_KEY}{id}";
        var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
        if (cachedProduct != null)
            return cachedProduct;

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return null;

        var productDto = MapToDto(product);
        await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(15));
        return productDto;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price
        };
    }
} 