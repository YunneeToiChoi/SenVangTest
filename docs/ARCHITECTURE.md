# 🏗️ System Architecture

## 📊 Clean Architecture với DDD

### 🧱 Project Structure
```
src/
├── OrderManagement.Domain/        # 🧱 Entities, Repository Interfaces
├── OrderManagement.Application/   # 🏢 DTOs, Services, Business Logic  
├── OrderManagement.Infrastructure/# 🔧 Database, Redis, Implementations
└── OrderManagement.API/           # 🌐 Controllers, Swagger, DI

tests/
└── OrderManagement.Tests/         # 🧪 Unit Tests với xUnit + Moq
```

### 🔄 Dependency Flow
```
API → Application → Domain ← Infrastructure
```

**Rules:**
- **API** chỉ phụ thuộc Application
- **Application** chỉ phụ thuộc Domain  
- **Infrastructure** implement interfaces từ Application
- **Domain** không phụ thuộc gì (pure business logic)

---

## 📦 Tech Stack Details

### API Layer (OrderManagement.API)
```csharp
✅ ASP.NET Core 8 Web API
✅ Swagger/OpenAPI documentation
✅ CORS configuration
✅ Dependency Injection container
✅ Error handling middleware
```

### Application Layer (OrderManagement.Application)
```csharp
✅ DTOs với DataAnnotation validation
✅ Service interfaces & implementations
✅ Business logic & rules
✅ ICacheService interface
```

### Infrastructure Layer (OrderManagement.Infrastructure)
```csharp
✅ Entity Framework Core 8
✅ SQL Server repositories
✅ Redis caching implementation
✅ Database context & configuration
```

### Domain Layer (OrderManagement.Domain)
```csharp
✅ Pure entities (Customer, Product, Order, OrderItem)
✅ Repository interfaces
✅ Domain models
✅ Business rules validation
```

---

## 🏛️ Database Design

### Entity Relationships
```
Customer (1) ────→ (*) Order (1) ────→ (*) OrderItem (*) ←──── (1) Product
```

### Tables
```sql
Customers:
- CustomerId (PK)
- FullName, Address, PhoneNumber (UNIQUE)

Products:
- ProductId (PK) 
- Name, Price

Orders:
- OrderId (PK)
- CustomerId (FK), OrderDate, TotalAmount

OrderItems:
- OrderItemId (PK)
- OrderId (FK), ProductId (FK), Quantity, UnitPrice
```

---

## ⚡ Performance Strategy

### Redis Caching
```yaml
Cache Strategy:
  - Customers: 10 minutes TTL (ít thay đổi)
  - Products:  15 minutes TTL (ít thay đổi nhất)  
  - Orders:    5 minutes TTL (thay đổi nhiều)
  
Cache Keys:
  - "customers:all"
  - "customer:{id}"
  - "products:all" 
  - "product:{id}"
  - "orders:all"
  - "order:{id}"
```

### Business Logic Optimizations
```csharp
✅ Async/Await toàn bộ operations
✅ Include() statements cho related data
✅ Connection pooling SQL Server
✅ Lazy loading với Redis
✅ Auto-calculate business rules
```

---

## 🔧 Configuration Management

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection",
    "Redis": "Redis connection"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Dependency Injection
```csharp
// Services registration
services.AddScoped<ICustomerService, CustomerService>();
services.AddScoped<IProductService, ProductService>();
services.AddScoped<IOrderService, OrderService>();

// Repositories
services.AddScoped<ICustomerRepository, CustomerRepository>();
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IOrderRepository, OrderRepository>();

// Cache
services.AddSingleton<ICacheService, RedisCacheService>();
```

---

## 🧪 Testing Architecture

### Unit Tests Structure
```
tests/OrderManagement.Tests/
├── Services/
│   ├── CustomerServiceTests.cs    # Service layer tests
│   ├── OrderServiceTests.cs       # Business logic tests
│   └── ProductServiceTests.cs     # Cache integration tests
└── Mocks/
    └── MockData.cs                # Test data & mocks
```

### Testing Strategy
```csharp
✅ Service layer unit tests
✅ Repository mocking với Moq
✅ Business logic validation tests
✅ Error handling scenarios
✅ Cache integration tests
```

---

## 🚀 Deployment Architecture

### Docker Compose Services
```yaml
services:
  sqlserver:        # SQL Server 2022 (Port 1433)
  redis:           # Redis 7 (Port 6379)
  # API runs outside Docker for development
```

### Development vs Production
```bash
Development:
✅ API runs on host (easier debugging)
✅ SQL Server + Redis in containers
✅ Swagger UI enabled
✅ Detailed logging

Production (future):
🔄 All services containerized
🔄 Load balancer
🔄 Redis cluster
🔄 SQL Server high availability
```

---

## 📈 Scalability Considerations

### Current Architecture Benefits
- ✅ **Stateless API** - easy horizontal scaling
- ✅ **Redis caching** - reduced database load
- ✅ **Clean separation** - maintainable codebase
- ✅ **Async operations** - better throughput

### Future Enhancements
- 🔄 **API Gateway** for multiple services
- 🔄 **Message Queue** for async processing
- 🔄 **CQRS pattern** for read/write separation
- 🔄 **Event Sourcing** for audit trails

---

## 🔒 Security & Best Practices

### Current Implementation
```csharp
✅ Input validation với DataAnnotations
✅ SQL injection protection (EF Core)
✅ CORS configuration
✅ Proper error handling
✅ Connection string security
```

### Security Recommendations
```csharp
🔄 JWT Authentication
🔄 Rate limiting
🔄 Input sanitization
🔄 HTTPS enforcement
🔄 API versioning
```

---

## 📚 Related Documentation

- 🚀 [Setup Guide](SETUP.md) - Cách cài đặt và chạy
- 📋 [API Documentation](API.md) - Chi tiết các endpoints
- 🔧 [Configuration](CONFIG.md) - Cấu hình hệ thống
- 🧪 [Testing](TESTING.md) - Unit tests và Postman

**🏗️ Architecture designed for maintainability, scalability, and testability!** 