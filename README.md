# 🛒 Hệ Thống Quản Lý Đơn Hàng (Order Management System)

Hệ thống quản lý đơn hàng được xây dựng với **ASP.NET Core 8**, **Entity Framework Core**, **Redis**, và **Docker** theo kiến trúc **Clean Architecture với Domain-Driven Design (DDD)**.

## 🏗️ Kiến Trúc & Công Nghệ

### Clean Architecture với DDD
```
src/
├── OrderManagement.Domain/        # 🧱 Domain Entities & Repository Interfaces
├── OrderManagement.Application/   # 🏢 Business Logic, DTOs, Services & Interfaces  
├── OrderManagement.Infrastructure/# 🔧 Database, Redis, Repository Implementations
└── OrderManagement.API/           # 🌐 Controllers, Swagger, Dependency Injection

tests/
└── OrderManagement.Tests/         # 🧪 Unit Tests với xUnit + Moq
```

### 💻 Stack Công Nghệ
- **.NET 8.0** - Framework chính
- **ASP.NET Core 8** - Web API với Swagger/OpenAPI
- **Entity Framework Core 8** - ORM Code-First approach
- **SQL Server 2022** - Database chính với seed data
- **Redis 7** - Caching layer cho performance
- **Docker & Docker Compose** - Container orchestration
- **xUnit + Moq** - Unit testing framework
- **Clean Architecture** - Dependency Inversion Principle

## 📊 Database Schema

### Entity Relationships
```
Customer (1) ────→ (*) Order (1) ────→ (*) OrderItem (*) ←──── (1) Product
```

### Tables & Constraints
**🧑‍💼 Customers**
```sql
CustomerId      INT PRIMARY KEY IDENTITY
FullName        NVARCHAR(100) NOT NULL
Address         NVARCHAR(200) NOT NULL  
PhoneNumber     NVARCHAR(15) NOT NULL UNIQUE ⭐ -- Unique constraint
```

**📦 Products**
```sql
ProductId       INT PRIMARY KEY IDENTITY
Name            NVARCHAR(100) NOT NULL
Price           DECIMAL(18,2) NOT NULL
```

**📋 Orders**
```sql
OrderId         INT PRIMARY KEY IDENTITY
CustomerId      INT FOREIGN KEY
OrderDate       DATETIME2 NOT NULL
TotalAmount     DECIMAL(18,2) NOT NULL
```

**📦 OrderItems**
```sql
OrderItemId     INT PRIMARY KEY IDENTITY
OrderId         INT FOREIGN KEY
ProductId       INT FOREIGN KEY
Quantity        INT NOT NULL
UnitPrice       DECIMAL(18,2) NOT NULL
```

## 🚀 API Documentation

### Base URLs
```
HTTP:        http://localhost:5246/api
HTTPS:       https://localhost:7181/api
Swagger UI:  http://localhost:5246/swagger (recommended)
```

### 👥 Customer Management API

#### **GET /api/customers**
Lấy danh sách tất cả khách hàng (với Redis caching)
```http
GET http://localhost:5246/api/customers
Accept: application/json
```
**Response 200:**
```json
[
  {
    "customerId": 1,
    "fullName": "Nguyễn Văn A",
    "address": "123 Đường ABC, TP.HCM",
    "phoneNumber": "0901234567"
  }
]
```

#### **GET /api/customers/{id}**
Lấy thông tin khách hàng theo ID
```http
GET http://localhost:5246/api/customers/1
```
**Response 200:** Customer object
**Response 404:** Customer không tồn tại

#### **POST /api/customers**
Tạo khách hàng mới (với unique phone validation)
```http
POST http://localhost:5246/api/customers
Content-Type: application/json

{
  "fullName": "Trần Thị B",
  "address": "456 Đường XYZ, Hà Nội", 
  "phoneNumber": "0987654321"
}
```
**Response 201:** Customer created
**Response 400:** Duplicate phone number

#### **PUT /api/customers/{id}**
Cập nhật thông tin khách hàng
```http
PUT http://localhost:5246/api/customers/1
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A Updated",
  "address": "789 Đường DEF, TP.HCM",
  "phoneNumber": "0901111111"
}
```

#### **DELETE /api/customers/{id}**
Xóa khách hàng
```http
DELETE http://localhost:5246/api/customers/1
```
**Response 204:** Xóa thành công
**Response 404:** Customer không tồn tại

### 📦 Product Management API

#### **GET /api/products**
Lấy danh sách tất cả sản phẩm (với Redis caching)
```http
GET http://localhost:5246/api/products
```
**Response:**
```json
[
  {
    "productId": 1,
    "name": "Laptop Dell",
    "price": 15000000
  },
  {
    "productId": 2,
    "name": "Mouse Logitech", 
    "price": 500000
  }
]
```

#### **GET /api/products/{id}**
Lấy thông tin sản phẩm theo ID
```http
GET http://localhost:5246/api/products/1
```

### 📋 Order Management API

#### **GET /api/orders**
Lấy danh sách đơn hàng (với filtering và caching)
```http
# Tất cả đơn hàng
GET http://localhost:5246/api/orders

# Lọc theo khách hàng
GET http://localhost:5246/api/orders?customerId=1

# Lọc theo ngày
GET http://localhost:5246/api/orders?fromDate=2024-12-01&toDate=2024-12-31

# Lọc kết hợp
GET http://localhost:5246/api/orders?customerId=1&fromDate=2024-12-01&toDate=2024-12-31
```

#### **GET /api/orders/{id}**
Lấy chi tiết đơn hàng
```http
GET http://localhost:5246/api/orders/1
```
**Response:**
```json
{
  "orderId": 1,
  "customerId": 1,
  "customerName": "Nguyễn Văn A",
  "orderDate": "2024-12-01T10:30:00",
  "totalAmount": 31000000,
  "orderItems": [
    {
      "orderItemId": 1,
      "productId": 1,
      "productName": "Laptop Dell",
      "quantity": 2,
      "unitPrice": 15000000
    },
    {
      "orderItemId": 2,
      "productId": 2,
      "productName": "Mouse Logitech",
      "quantity": 2,
      "unitPrice": 500000
    }
  ]
}
```

#### **POST /api/orders**
Tạo đơn hàng mới (với business logic validation)
```http
POST http://localhost:5246/api/orders
Content-Type: application/json

{
  "customerId": 1,
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```
**Business Logic:**
- ✅ Auto-calculate TotalAmount = sum(quantity × unitPrice)
- ✅ Validate Customer exists
- ✅ Validate tất cả Products exist
- ✅ Auto-set OrderDate = DateTime.Now

## ⚙️ Hướng Dẫn Cài Đặt & Chạy

### 📋 Prerequisites
```bash
1. .NET 8 SDK - https://dotnet.microsoft.com/download
2. Docker Desktop - https://www.docker.com/products/docker-desktop
3. Git
4. IDE: Visual Studio 2022 / VS Code / Rider
```

### 🔧 Bước 1: Clone & Setup
```bash
# Clone repository
git clone <repository-url>
cd OrderManagement

# Check .NET version
dotnet --version
# Phải >= 8.0.0
```

### 🐳 Bước 2: Start Infrastructure với Docker
```bash
# Start SQL Server + Redis
docker-compose up -d

# Verify services running
docker ps

# Expected output:
# - sqlserver_ordermanagement (port 1433)
# - redis_ordermanagement (port 6379)
```

**🔍 Kiểm tra services:**
```bash
# Test SQL Server connection
docker exec -it sqlserver_ordermanagement /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TestSenVang@Password"

# Test Redis connection  
docker exec -it redis_ordermanagement redis-cli ping
# Expected: PONG
```

### 🏃‍♂️ Bước 3: Run Application

#### Option 1: Command Line
```bash
# Restore NuGet packages
dotnet restore

# Build solution
dotnet build

# Run API project
cd src/OrderManagement.API
dotnet run

# Hoặc với watch mode (auto-reload)
dotnet watch run
```

#### Option 2: Visual Studio
```bash
1. Open OrderManagement.sln
2. Set OrderManagement.API as Startup Project
3. Press F5 hoặc Ctrl+F5
```

### 🌐 Bước 4: Access Application
```bash
✅ HTTP API:       http://localhost:5246/api
✅ HTTPS API:      https://localhost:7181/api
✅ Swagger UI:     http://localhost:5246/swagger  
✅ Health Check:   http://localhost:5246/api/customers
```

**🎯 Test API nhanh:**
```bash
# Using curl
curl http://localhost:5246/api/customers

# Using PowerShell
Invoke-RestMethod -Uri "http://localhost:5246/api/customers"
```

## 🧪 Testing

### Unit Tests
```bash
# Run all tests
dotnet test

# Run với coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "CustomerServiceTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage
- ✅ **CustomerService**: CRUD + Caching scenarios
- ✅ **OrderService**: Order creation + Business validation
- ✅ **Repository Mocking**: Data access layer
- ✅ **Error Handling**: Exception scenarios

### Manual Testing với Swagger
1. Mở http://localhost:5246/swagger
2. Test **GET /api/products** (xem seed data)
3. Test **POST /api/customers** (tạo customer mới)
4. Test **POST /api/orders** (tạo order với customer + products)

## 🚀 Performance Features

### 🔥 Redis Caching Strategy
```yaml
Cache TTL Settings:
  - Customers:     10 minutes  # Ít thay đổi
  - Products:      15 minutes  # Ít thay đổi nhất  
  - Orders:        5 minutes   # Thay đổi nhiều
  - Cache Keys:    "customer:1", "products:all", "order:123"
```

**Cache Flow:**
```mermaid
graph LR
A[Request] --> B{Cache Hit?}
B -->|Yes| C[Return Cached Data]
B -->|No| D[Query Database]
D --> E[Cache Result]
E --> F[Return Data]
```

### ⚡ Performance Optimizations
- **Async/Await** pattern toàn bộ application
- **Connection pooling** cho SQL Server
- **Lazy loading** với Include() statements
- **Bulk operations** cho OrderItems
- **Memory optimization** với IEnumerable

## 🔧 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderManagementDB;User Id=sa;Password=TestSenVang@Password;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### docker-compose.yml Services
```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports: ["1433:1433"]
    environment:
      SA_PASSWORD: "TestSenVang@Password"
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Express"

  redis:
    image: redis:7-alpine  
    ports: ["6379:6379"]
```

## 📋 Seed Data

Khi chạy application lần đầu, database sẽ được tự động tạo với data mẫu:

### 👥 Sample Customers (3 customers với unique phones)
```
ID  | Tên           | Địa chỉ                    | SĐT        | Status
1   | Nguyễn Văn A  | 123 Đường ABC, TP.HCM     | 0901234567 | ✅ Unique
2   | Trần Thị B    | 456 Đường XYZ, Hà Nội     | 0987654321 | ✅ Unique
3   | Lê Văn C      | 789 Đường DEF, Đà Nẵng    | 0912345678 | ✅ Unique
```

### 📦 Sample Products  
```
ID  | Tên sản phẩm         | Giá (VND)
1   | Laptop Dell          | 15,000,000
2   | Mouse Logitech       | 500,000  
3   | Keyboard Mechanical  | 1,200,000
4   | Monitor Samsung      | 8,000,000
```

### 📋 Sample Orders với OrderItems
```
Order 1 (Nguyễn Văn A - 2024-12-01):
├── 2x Laptop Dell (15M each) = 30M
├── 2x Mouse Logitech (500K each) = 1M
└── Total: 31,000,000 VND

Order 2 (Trần Thị B - 2024-12-02):
├── 1x Monitor Samsung (8M) = 8M  
├── 1x Keyboard Mechanical (1.2M) = 1.2M
└── Total: 9,200,000 VND

Order 3 (Nguyễn Văn A - 2024-12-03):
├── 1x Keyboard Mechanical (1.2M) = 1.2M
├── 1x Mouse Logitech (500K) = 500K
└── Total: 1,700,000 VND
```

## 🛠️ Troubleshooting

### ❌ Common Issues

**1. SQL Server Login Failed**
```bash
Error: Login failed for user 'sa'
```
**Solutions:**
```bash
# Check password in appsettings.json và docker-compose.yml
# Phải giống nhau: "TestSenVang@Password"

# Reset SQL Server container
docker-compose down
docker volume rm senvangtest_sqlserver_data
docker-compose up -d

# Wait 15-20 seconds for SQL Server to initialize
```

**2. Port Already in Use**
```bash
# Check what's using port 5246
netstat -ano | findstr :5246

# Kill process
taskkill /PID <process-id> /F

# Or change port in launchSettings.json
```

**3. Redis Connection Failed**
```bash
# Check Redis container  
docker logs redis_ordermanagement

# Test Redis
docker exec -it redis_ordermanagement redis-cli ping
```

**4. Docker Issues**
```bash
# Reset Docker containers
docker-compose down
docker-compose up -d --force-recreate

# Clear Docker cache
docker system prune -a
```

**5. Database Schema Issues**
```bash
# Reset database với seed data mới
docker-compose down
docker volume rm senvangtest_sqlserver_data
docker-compose up -d

# Database sẽ tự động tạo với schema mới
```

### 📊 Health Checks
```bash
# API Health (should return customers)
curl http://localhost:5246/api/customers

# Database Health (should return products)
curl http://localhost:5246/api/products

# Redis Health (check logs)
docker logs redis_ordermanagement | grep "Ready to accept connections"

# SQL Server Health  
docker exec sqlserver_ordermanagement /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TestSenVang@Password" -Q "SELECT @@VERSION"
```

## 🏆 Architecture Benefits

### ✅ Clean Architecture Advantages
- **Testability**: Dễ test với dependency injection
- **Maintainability**: Tách biệt concerns rõ ràng
- **Scalability**: Dễ extend functionality
- **Flexibility**: Swap implementations (Redis → MemoryCache)

### 🔄 Dependency Flow
```
API → Application → Domain ← Infrastructure
```
- **API** chỉ phụ thuộc Application
- **Application** chỉ phụ thuộc Domain  
- **Infrastructure** implement interfaces từ Application
- **Domain** không phụ thuộc gì (pure business logic)

## 📝 Development Notes

### Useful Commands
```bash
# Build specific project
dotnet build src/OrderManagement.API

# Watch specific project  
dotnet watch --project src/OrderManagement.API

# Add NuGet package
dotnet add src/OrderManagement.Infrastructure package StackExchange.Redis

# Entity Framework
dotnet ef database update --project src/OrderManagement.Infrastructure

# Generate migration
dotnet ef migrations add InitialCreate --project src/OrderManagement.Infrastructure
```

### Code Quality
- ✅ **Consistent naming** theo C# conventions
- ✅ **Async/await** cho tất cả I/O operations  
- ✅ **Error handling** với proper HTTP status codes
- ✅ **Validation** ở multiple layers (unique phone constraint)
- ✅ **Logging** configured sẵn
- ✅ **CORS** enabled cho frontend development

## 🎯 Business Rules

### Customer Management
- ✅ **Phone number must be unique** (database constraint)
- ✅ All fields required (FullName, Address, PhoneNumber)
- ✅ Phone number max 15 characters

### Order Management  
- ✅ **Auto-calculate total amount** from OrderItems
- ✅ **Validate customer exists** before creating order
- ✅ **Validate all products exist** before creating order
- ✅ **Auto-set order date** to current timestamp
- ✅ **Include customer and product names** in responses

---

## 📞 Support & Quick Start

### 🚀 Quick Start Checklist
```bash
1. ✅ Clone repository
2. ✅ Start Docker: docker-compose up -d
3. ✅ Run API: dotnet run --project src/OrderManagement.API
4. ✅ Test: http://localhost:5246/swagger
5. ✅ Verify seed data: GET /api/customers, /api/products, /api/orders
```

### 🔍 Nếu gặp vấn đề, check:
1. ✅ .NET 8 SDK installed
2. ✅ Docker Desktop running
3. ✅ Ports 1433, 6379, 5246 không bị conflict
4. ✅ Password "TestSenVang@Password" trong cả 2 files
5. ✅ Swagger UI hoạt động: http://localhost:5246/swagger

**Happy Coding! 🚀** 