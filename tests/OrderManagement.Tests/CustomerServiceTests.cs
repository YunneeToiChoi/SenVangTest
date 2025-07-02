using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using Moq;
using Xunit;

namespace OrderManagement.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _customerService = new CustomerService(
            _mockCustomerRepository.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerExists_ReturnsCustomerDto()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            CustomerId = customerId,
            FullName = "Nguyễn Văn A",
            Address = "123 ABC Street",
            PhoneNumber = "0901234567"
        };

        _mockCacheService.Setup(x => x.GetAsync<CustomerDto>(It.IsAny<string>()))
            .ReturnsAsync((CustomerDto?)null);
        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal("Nguyễn Văn A", result.FullName);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerNotExists_ReturnsNull()
    {
        // Arrange
        var customerId = 999;
        _mockCacheService.Setup(x => x.GetAsync<CustomerDto>(It.IsAny<string>()))
            .ReturnsAsync((CustomerDto?)null);
        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _customerService.GetCustomerByIdAsync(customerId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomerAsync_ValidInput_ReturnsCustomerDto()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FullName = "Trần Thị B",
            Address = "456 XYZ Street",
            PhoneNumber = "0987654321"
        };

        var createdCustomer = new Customer
        {
            CustomerId = 1,
            FullName = createDto.FullName,
            Address = createDto.Address,
            PhoneNumber = createDto.PhoneNumber
        };

        _mockCustomerRepository.Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync(createdCustomer);

        // Act
        var result = await _customerService.CreateCustomerAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CustomerId);
        Assert.Equal("Trần Thị B", result.FullName);
    }
} 