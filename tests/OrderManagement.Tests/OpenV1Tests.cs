using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using Moq;
using Xunit;

namespace OrderManagement.Tests;

/// <summary>
/// Test suite for "open v1" functionality - validates core system operations
/// This test suite verifies that the Order Management System v1 is working properly
/// </summary>
public class OpenV1Tests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly CustomerService _customerService;
    private readonly ProductService _productService;
    private readonly OrderService _orderService;

    public OpenV1Tests()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCacheService = new Mock<ICacheService>();
        
        _customerService = new CustomerService(
            _mockCustomerRepository.Object,
            _mockCacheService.Object);
            
        _productService = new ProductService(
            _mockProductRepository.Object,
            _mockCacheService.Object);
            
        _orderService = new OrderService(
            _mockOrderRepository.Object,
            _mockCustomerRepository.Object,
            _mockProductRepository.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task OpenV1_SystemCanRetrieveCustomers_Success()
    {
        // Arrange - Test that customer retrieval is working (open to customer data)
        var customers = new List<Customer>
        {
            new Customer 
            { 
                CustomerId = 1, 
                FullName = "Test Customer 1", 
                Address = "123 Test Street",
                PhoneNumber = "0901234567"
            },
            new Customer 
            { 
                CustomerId = 2, 
                FullName = "Test Customer 2", 
                Address = "456 Test Avenue",
                PhoneNumber = "0987654321"
            }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<CustomerDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<CustomerDto>?)null);
        _mockCustomerRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _customerService.GetAllCustomersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.FullName == "Test Customer 1");
        Assert.Contains(result, c => c.FullName == "Test Customer 2");
    }

    [Fact]
    public async Task OpenV1_SystemCanRetrieveProducts_Success()
    {
        // Arrange - Test that product retrieval is working (open to product catalog)
        var products = new List<Product>
        {
            new Product 
            { 
                ProductId = 1, 
                Name = "Test Product 1", 
                Price = 100000
            },
            new Product 
            { 
                ProductId = 2, 
                Name = "Test Product 2", 
                Price = 200000
            }
        };

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<ProductDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<ProductDto>?)null);
        _mockProductRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Test Product 1");
        Assert.Contains(result, p => p.Name == "Test Product 2");
    }

    [Fact]
    public async Task OpenV1_SystemCanCreateAndRetrieveOrders_Success()
    {
        // Arrange - Test that order creation and retrieval is working (open to order processing)
        var createOrderDto = new CreateOrderDto
        {
            CustomerId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = 1, Quantity = 2 },
                new CreateOrderItemDto { ProductId = 2, Quantity = 1 }
            }
        };

        var customer = new Customer 
        { 
            CustomerId = 1, 
            FullName = "Test Customer",
            Address = "123 Test Street",
            PhoneNumber = "0901234567"
        };

        var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Product 1", Price = 50000 },
            new Product { ProductId = 2, Name = "Product 2", Price = 75000 }
        };

        var createdOrder = new Order
        {
            OrderId = 1,
            CustomerId = 1,
            OrderDate = DateTime.Now,
            TotalAmount = 175000,
            Customer = customer,
            OrderItems = new List<OrderItem>
            {
                new OrderItem 
                { 
                    OrderItemId = 1, 
                    ProductId = 1, 
                    Quantity = 2, 
                    UnitPrice = 50000,
                    Product = products[0]
                },
                new OrderItem 
                { 
                    OrderItemId = 2, 
                    ProductId = 2, 
                    Quantity = 1, 
                    UnitPrice = 75000,
                    Product = products[1]
                }
            }
        };

        // Setup mocks
        _mockCustomerRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        _mockProductRepository.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(products);
        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(createdOrder);
        _mockOrderRepository.Setup(x => x.GetByIdWithItemsAsync(1))
            .ReturnsAsync(createdOrder);
        _mockCacheService.Setup(x => x.GetAsync<OrderDto>(It.IsAny<string>()))
            .ReturnsAsync((OrderDto?)null);

        // Act - Create order
        var createResult = await _orderService.CreateOrderAsync(createOrderDto);

        // Act - Retrieve order
        var retrieveResult = await _orderService.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(createResult);
        Assert.Equal(1, createResult.OrderId);
        Assert.Equal(175000, createResult.TotalAmount);
        Assert.Equal(2, createResult.OrderItems.Count());
        
        Assert.NotNull(retrieveResult);
        Assert.Equal(1, retrieveResult.OrderId);
        Assert.Equal("Test Customer", retrieveResult.CustomerName);
        Assert.Equal(2, retrieveResult.OrderItems.Count());
    }

    [Fact]
    public async Task OpenV1_SystemHandlesEmptyResults_Success()
    {
        // Arrange - Test that system gracefully handles empty results (open to no data scenarios)
        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<CustomerDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<CustomerDto>?)null);
        _mockCustomerRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Customer>());

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<ProductDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<ProductDto>?)null);
        _mockProductRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>());

        _mockCacheService.Setup(x => x.GetAsync<IEnumerable<OrderDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<OrderDto>?)null);
        _mockOrderRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Order>());

        // Act
        var customers = await _customerService.GetAllCustomersAsync();
        var products = await _productService.GetAllProductsAsync();
        var orders = await _orderService.GetAllOrdersAsync();

        // Assert
        Assert.NotNull(customers);
        Assert.Empty(customers);
        
        Assert.NotNull(products);
        Assert.Empty(products);
        
        Assert.NotNull(orders);
        Assert.Empty(orders);
    }

    [Fact]
    public void OpenV1_CoreServicesAreProperlyConstructed_Success()
    {
        // Arrange & Act - Test that all core services can be instantiated (system is open to dependency injection)
        
        // Assert
        Assert.NotNull(_customerService);
        Assert.NotNull(_productService);
        Assert.NotNull(_orderService);
        
        // Verify services implement the expected interfaces
        Assert.IsAssignableFrom<ICustomerService>(_customerService);
        Assert.IsAssignableFrom<IProductService>(_productService);
        Assert.IsAssignableFrom<IOrderService>(_orderService);
    }

    [Fact]
    public async Task OpenV1_SystemValidatesBusinessRules_Success()
    {
        // Arrange - Test that system enforces business rules (open to validation)
        var invalidOrderDto = new CreateOrderDto
        {
            CustomerId = 999, // Non-existent customer
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = 1, Quantity = 1 }
            }
        };

        _mockCustomerRepository.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _orderService.CreateOrderAsync(invalidOrderDto));
    }
}