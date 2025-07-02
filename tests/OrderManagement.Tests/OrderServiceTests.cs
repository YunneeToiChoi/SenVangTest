using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using Moq;
using Xunit;

namespace OrderManagement.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _orderService = new OrderService(
            _mockOrderRepository.Object,
            _mockCustomerRepository.Object,
            _mockProductRepository.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ReturnsOrderDto()
    {
        // Arrange
        var orderId = 1;
        var order = new Order
        {
            OrderId = orderId,
            CustomerId = 1,
            OrderDate = DateTime.Now,
            TotalAmount = 1000000,
            Customer = new Customer { CustomerId = 1, FullName = "Test Customer" },
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 500000,
                    Product = new Product { ProductId = 1, Name = "Test Product" }
                }
            }
        };

        _mockCacheService.Setup(x => x.GetAsync<OrderDto>(It.IsAny<string>()))
            .ReturnsAsync((OrderDto?)null);
        _mockOrderRepository.Setup(x => x.GetByIdWithItemsAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(1000000, result.TotalAmount);
        Assert.Single(result.OrderItems);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidInput_ReturnsOrderDto()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            CustomerId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = 1, Quantity = 2 }
            }
        };

        _mockCustomerRepository.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _mockProductRepository.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new List<Product>
            {
                new Product { ProductId = 1, Name = "Test Product", Price = 500000 }
            });

        var createdOrder = new Order
        {
            OrderId = 1,
            CustomerId = 1,
            OrderDate = DateTime.Now,
            TotalAmount = 1000000,
            OrderItems = new List<OrderItem>()
        };

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(createdOrder);
        _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Customer { CustomerId = 1, FullName = "Test Customer" });

        // Act
        var result = await _orderService.CreateOrderAsync(createOrderDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.OrderId);
        Assert.Equal(1000000, result.TotalAmount);
    }

    [Fact]
    public async Task CreateOrderAsync_CustomerNotExists_ThrowsArgumentException()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            CustomerId = 999,
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = 1, Quantity = 2 }
            }
        };

        _mockCustomerRepository.Setup(x => x.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _orderService.CreateOrderAsync(createOrderDto));
    }
} 