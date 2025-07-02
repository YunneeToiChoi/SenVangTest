# 🛒 Hệ Thống Quản Lý Đơn Hàng

**Order Management System** được xây dựng với **ASP.NET Core 8**, **Redis**, **SQL Server** theo kiến trúc **Clean Architecture với DDD**.

---

## 🚀 Quick Start (5 phút)

### 📋 Prerequisites
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)

### ⚡ Chạy ngay
```bash
# 1. Clone project
git clone <repository-url>
cd OrderManagement

# 2. Start infrastructure
docker-compose up -d

# 3. Run API
cd src/OrderManagement.API
dotnet run

# 4. Test API
# Mở: http://localhost:5246/swagger
```

### 🌐 URLs sau khi chạy
```
✅ Swagger UI:     http://localhost:5246/swagger
✅ HTTP API:       http://localhost:5246/api
✅ HTTPS API:      https://localhost:7181/api
✅ Health Check:   http://localhost:5246/api/customers
```

---

## 📚 Documentation

| 📖 Tài liệu | 📝 Nội dung |
|-------------|-------------|
| **[🚀 Cách chạy chi tiết](docs/SETUP.md)** | Hướng dẫn cài đặt, troubleshooting, Docker |
| **[📋 API Documentation](docs/API.md)** | Tất cả endpoints, examples, validation |
| **[🏗️ Architecture](docs/ARCHITECTURE.md)** | Clean Architecture, DDD, tech stack |
| **[🔧 Configuration](docs/CONFIG.md)** | appsettings, Docker, database, Redis |
| **[🧪 Testing](docs/TESTING.md)** | Unit tests, Postman collection |

---

## 🎯 Core Features

### ✅ **Business Logic**
- **CRUD Customers** với unique phone validation
- **Read Products** từ seed data
- **Order Management** với auto-calculate total
- **Filtering** orders theo customer & date range

### ⚡ **Performance** 
- **Redis Caching** (customers: 10min, products: 15min, orders: 5min)
- **Async/Await** toàn bộ operations
- **Connection pooling** SQL Server

### 🏗️ **Architecture**
- **Clean Architecture** với DDD pattern
- **Dependency Injection** hoàn chỉnh
- **Validation** với DataAnnotations
- **Error Handling** với proper HTTP codes

---

## 📦 Tech Stack

| Layer | Technology |
|-------|------------|
| **API** | ASP.NET Core 8, Swagger/OpenAPI |
| **Business** | Clean Architecture, DDD, AutoMapper |
| **Data** | Entity Framework Core 8, SQL Server 2022 |
| **Cache** | Redis 7 với StackExchange.Redis |
| **Testing** | xUnit, Moq, Integration Tests |
| **DevOps** | Docker Compose, Containerization |

---

## 🔍 Quick Test

### API Health Check
```bash
# Test customers (should return 3 customers)
curl http://localhost:5246/api/customers

# Test products (should return 4 products)  
curl http://localhost:5246/api/products

# Test orders (should return 3 orders with details)
curl http://localhost:5246/api/orders
```

### Postman Collection
📁 **File:** `postman_collection.json`
- Import vào Postman để test tất cả endpoints
- Có sẵn sample data và validation examples

---

## 🛠️ Troubleshooting Nhanh

### ❌ Common Issues

**SQL Server Login Failed:**
```bash
# Reset password mismatch
docker-compose down
docker volume rm senvangtest_sqlserver_data  
docker-compose up -d
```

**Port 5246 đã bị dùng:**
```bash
# Check process
netstat -ano | findstr :5246
# Kill process  
taskkill /PID <process-id> /F
```

**Redis Connection Error:**
```bash
# Test Redis
docker exec -it redis_ordermanagement redis-cli ping
# Expected: PONG
```

---

## 📞 Support

### 🚀 **Quick Help Checklist:**
1. ✅ .NET 8 SDK installed?
2. ✅ Docker Desktop running?
3. ✅ Ports 1433, 6379, 5246 free?
4. ✅ Password "TestSenVang@Password" matched?
5. ✅ Swagger working: http://localhost:5246/swagger?

### 📧 **Need Help?**
- 📋 Check [Setup Guide](docs/SETUP.md) for detailed instructions
- 🔍 Check [API Docs](docs/API.md) for endpoint examples
- 🏗️ Check [Architecture](docs/ARCHITECTURE.md) for system design

---

**🎉 Happy Coding!** 

*Phiên bản gọn nhẹ này giúp bạn chạy system trong 5 phút. Xem docs folder để có thông tin chi tiết hơn.* 