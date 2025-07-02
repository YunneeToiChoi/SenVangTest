# 🧪 Testing Guide

## 🏗️ Testing Architecture

### Test Structure
```
tests/OrderManagement.Tests/
├── Services/
│   ├── CustomerServiceTests.cs    # Customer service unit tests
│   ├── OrderServiceTests.cs       # Order service + business logic
│   └── ProductServiceTests.cs     # Product service + caching
├── Mocks/
│   └── MockData.cs                # Test data và mock setup
└── OrderManagement.Tests.csproj   # Test project file
```

### Testing Framework
```csharp
✅ xUnit - Testing framework
✅ Moq - Mocking repositories
✅ Microsoft.EntityFrameworkCore.InMemory - In-memory database
✅ FluentAssertions - Readable assertions (optional)
```

---

## 🚀 Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run với detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "CustomerServiceTests"

# Run specific test method
dotnet test --filter "GetAllCustomersAsync_ShouldReturnAllCustomers"

# Run với coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio
```bash
1. Open Test Explorer (Test → Test Explorer)
2. Build solution
3. Click "Run All Tests"
4. Or right-click specific tests to run
```

### Watch Mode
```bash
# Auto-run tests on code changes
dotnet watch test --project tests/OrderManagement.Tests
```

---

## 🧪 Unit Tests Examples

### CustomerService Tests
```csharp
[Fact]
public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
{
    // Arrange
    var customers = MockData.GetTestCustomers();
    _mockCustomerRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(customers);

    // Act
    var result = await _customerService.GetAllCustomersAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(3, result.Count());
    _mockCacheService.Verify(c => c.SetAsync(It.IsAny<string>(), 
                                           It.IsAny<object>(), 
                                           It.IsAny<TimeSpan>()), Times.Once);
}

[Fact]
public async Task CreateCustomerAsync_WithDuplicatePhone_ShouldThrowException()
{
    // Arrange
    var newCustomer = new CreateCustomerDto 
    { 
        FullName = "Test User", 
        Address = "Test Address", 
        PhoneNumber = "0901234567" // Duplicate phone
    };
    
    _mockCustomerRepository.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                          .ReturnsAsync(true);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() => 
        _customerService.CreateCustomerAsync(newCustomer));
}
```

### OrderService Tests
```csharp
[Fact]
public async Task CreateOrderAsync_ShouldCalculateTotalAmount()
{
    // Arrange
    var createOrderDto = new CreateOrderDto
    {
        CustomerId = 1,
        OrderItems = new List<CreateOrderItemDto>
        {
            new() { ProductId = 1, Quantity = 2 }, // Laptop: 2 x 15M = 30M
            new() { ProductId = 2, Quantity = 1 }  // Mouse: 1 x 500K = 500K
        }
    };
    
    SetupMockData();

    // Act
    var result = await _orderService.CreateOrderAsync(createOrderDto);

    // Assert
    Assert.Equal(30500000, result.TotalAmount); // 30M + 500K
    Assert.Equal(2, result.OrderItems.Count);
    Assert.True(result.OrderDate <= DateTime.Now);
}

[Fact]
public async Task CreateOrderAsync_WithInvalidCustomer_ShouldThrowException()
{
    // Arrange
    var createOrderDto = new CreateOrderDto
    {
        CustomerId = 999, // Non-existent customer
        OrderItems = new List<CreateOrderItemDto>
        {
            new() { ProductId = 1, Quantity = 1 }
        }
    };

    _mockCustomerRepository.Setup(r => r.GetByIdAsync(999))
                          .ReturnsAsync((Customer)null);

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() => 
        _orderService.CreateOrderAsync(createOrderDto));
}
```

### Cache Integration Tests
```csharp
[Fact]
public async Task GetProductsAsync_ShouldUseCache()
{
    // Arrange
    var products = MockData.GetTestProducts();
    _mockCacheService.Setup(c => c.GetAsync<IEnumerable<ProductDto>>("products:all"))
                    .ReturnsAsync((IEnumerable<ProductDto>)null); // Cache miss
    
    _mockProductRepository.Setup(r => r.GetAllAsync())
                         .ReturnsAsync(products);

    // Act
    var result = await _productService.GetAllProductsAsync();

    // Assert
    Assert.Equal(4, result.Count());
    
    // Verify cache was set
    _mockCacheService.Verify(c => c.SetAsync("products:all", 
                                           It.IsAny<object>(), 
                                           TimeSpan.FromMinutes(15)), Times.Once);
}
```

---

## 📊 Mock Data Setup

### MockData.cs
```csharp
public static class MockData
{
    public static List<Customer> GetTestCustomers()
    {
        return new List<Customer>
        {
            new() { CustomerId = 1, FullName = "Nguyễn Văn A", Address = "TP.HCM", PhoneNumber = "0901234567" },
            new() { CustomerId = 2, FullName = "Trần Thị B", Address = "Hà Nội", PhoneNumber = "0987654321" },
            new() { CustomerId = 3, FullName = "Lê Văn C", Address = "Đà Nẵng", PhoneNumber = "0912345678" }
        };
    }

    public static List<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new() { ProductId = 1, Name = "Laptop Dell", Price = 15000000 },
            new() { ProductId = 2, Name = "Mouse Logitech", Price = 500000 },
            new() { ProductId = 3, Name = "Keyboard Mechanical", Price = 1200000 },
            new() { ProductId = 4, Name = "Monitor Samsung", Price = 8000000 }
        };
    }

    public static List<Order> GetTestOrders()
    {
        return new List<Order>
        {
            new() 
            { 
                OrderId = 1, 
                CustomerId = 1, 
                OrderDate = DateTime.Parse("2024-12-01T10:30:00"),
                TotalAmount = 31000000,
                OrderItems = new List<OrderItem>
                {
                    new() { OrderItemId = 1, ProductId = 1, Quantity = 2, UnitPrice = 15000000 },
                    new() { OrderItemId = 2, ProductId = 2, Quantity = 2, UnitPrice = 500000 }
                }
            }
        };
    }
}
```

---

## 📱 Postman Testing

### Import Collection
```bash
1. Open Postman
2. Click "Import" 
3. Select file: postman_collection.json
4. Collection "Order Management API" will appear
```

### Collection Structure
```
📁 Order Management API
├── 👥 Customer Management
│   ├── Get All Customers
│   ├── Get Customer by ID  
│   ├── Create Customer
│   ├── Update Customer
│   └── Delete Customer
├── 📦 Product Management
│   ├── Get All Products
│   └── Get Product by ID
├── 📋 Order Management
│   ├── Get All Orders
│   ├── Get Orders by Customer
│   ├── Get Orders by Date Range
│   ├── Get Orders - Combined Filter
│   ├── Get Order by ID
│   └── Create Order
└── 🔧 System Health
    ├── API Health Check
    └── Swagger Documentation
```

### Environment Variables
```bash
{{baseUrl}} = http://localhost:5246/api
```

### Test Scenarios

**1. Basic Health Check**
```bash
GET {{baseUrl}}/customers
Expected: 200 OK với 3 customers
```

**2. Create Customer with Validation**
```bash
POST {{baseUrl}}/customers
Body: {
  "fullName": "",  # Invalid: empty
  "address": "",   # Invalid: empty
  "phoneNumber": "invalid"  # Invalid: not phone format
}
Expected: 400 Bad Request với validation errors
```

**3. Create Order with Business Logic**
```bash
POST {{baseUrl}}/orders
Body: {
  "customerId": 1,
  "orderItems": [
    {"productId": 1, "quantity": 2},
    {"productId": 2, "quantity": 1}
  ]
}
Expected: 201 Created với auto-calculated total
```

---

## 🔍 Integration Testing

### API Integration Tests (future)
```csharp
public class CustomerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CustomerControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }
}
```

### Database Integration Tests
```csharp
public class DatabaseIntegrationTests
{
    private OrderManagementDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new OrderManagementDbContext(options);
    }

    [Fact]
    public async Task CanCreateAndRetrieveCustomer()
    {
        using var context = GetInMemoryDbContext();
        
        var customer = new Customer 
        { 
            FullName = "Test User", 
            Address = "Test Address", 
            PhoneNumber = "0999888777" 
        };
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        
        var retrieved = await context.Customers.FirstAsync();
        Assert.Equal("Test User", retrieved.FullName);
    }
}
```

---

## 📈 Test Coverage

### Coverage Tools
```bash
# Install coverage tool
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/*.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
```

### Current Test Coverage
```bash
✅ CustomerService: 90%+ coverage
  - CRUD operations
  - Cache integration
  - Validation scenarios
  - Error handling

✅ OrderService: 85%+ coverage  
  - Order creation business logic
  - Total amount calculation
  - Customer/Product validation
  - Error scenarios

✅ ProductService: 80%+ coverage
  - Read operations
  - Cache integration
  - Basic error handling
```

---

## 🚀 Testing Best Practices

### ✅ Do's
- ✅ Test business logic thoroughly
- ✅ Mock external dependencies
- ✅ Test error scenarios
- ✅ Use meaningful test names
- ✅ Follow AAA pattern (Arrange, Act, Assert)
- ✅ Test cache behavior
- ✅ Verify mock calls

### ❌ Don'ts
- ❌ Don't test framework code
- ❌ Don't write integration tests for everything
- ❌ Don't ignore test failures
- ❌ Don't test implementation details
- ❌ Don't use real database in unit tests

---

## 📚 Testing Checklist

### Before Deployment
```bash
✅ All unit tests pass
✅ Integration tests pass  
✅ Postman collection works
✅ API health checks pass
✅ Test coverage > 80%
✅ Performance tests (if any)
✅ Security tests (validation)
```

### Regular Testing
```bash
✅ Run tests before every commit
✅ Test new features thoroughly
✅ Update Postman collection for new endpoints
✅ Test error scenarios
✅ Verify cache behavior
```

---

## 📚 Related Documentation

- 🚀 [Setup Guide](SETUP.md) - Run tests in development
- 📋 [API Documentation](API.md) - API endpoint details  
- 🏗️ [Architecture](ARCHITECTURE.md) - System design
- 🔧 [Configuration](CONFIG.md) - Test configuration

**🧪 Well-tested code is reliable code!** 