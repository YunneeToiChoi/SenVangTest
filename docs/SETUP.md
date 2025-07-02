# 🚀 Hướng Dẫn Cài Đặt & Chạy

## 📋 Prerequisites

### 🔧 Công cụ bắt buộc
```bash
1. .NET 8 SDK - https://dotnet.microsoft.com/download
2. Docker Desktop - https://www.docker.com/products/docker-desktop
3. Git
4. IDE: Visual Studio 2022 / VS Code / Rider
```

### ✅ Kiểm tra version
```bash
# Check .NET version (phải >= 8.0.0)
dotnet --version

# Check Docker version
docker --version
docker-compose --version
```

---

## ⚡ Cách Chạy Nhanh (5 phút)

### 🔥 Option 1: Chạy trực tiếp
```bash
# 1. Clone repository
git clone <repository-url>
cd OrderManagement

# 2. Start infrastructure (SQL Server + Redis)
docker-compose up -d

# 3. Restore packages
dotnet restore

# 4. Run API
cd src/OrderManagement.API
dotnet run

# 5. Test
# Mở: http://localhost:5246/swagger
```

### 🛠️ Option 2: Visual Studio
```bash
1. Open OrderManagement.sln
2. Set OrderManagement.API as Startup Project
3. Ensure Docker is running
4. Run docker-compose up -d trong terminal
5. Press F5 hoặc Ctrl+F5
```

### 🔍 Option 3: Watch mode (auto-reload)
```bash
cd src/OrderManagement.API
dotnet watch run
```

---

## 🐳 Docker Setup Chi Tiết

### 📦 Services được start
```yaml
✅ SQL Server 2022 (Port 1433)
   - Database: OrderManagementDB
   - User: sa / Password: TestSenVang@Password
   
✅ Redis 7 (Port 6379)
   - No authentication
   - Used for caching
```

### 🔧 Docker Commands
```bash
# Start all services
docker-compose up -d

# Check running containers
docker ps

# View SQL Server logs
docker logs sqlserver_ordermanagement

# View Redis logs
docker logs redis_ordermanagement

# Stop all services
docker-compose down

# Reset data (clean start)
docker-compose down
docker volume rm senvangtest_sqlserver_data
docker-compose up -d
```

### 🔍 Verify Services
```bash
# Test SQL Server connection
docker exec -it sqlserver_ordermanagement /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TestSenVang@Password"

# Test Redis connection
docker exec -it redis_ordermanagement redis-cli ping
# Expected: PONG
```

---

## 🌐 URLs & Endpoints

### 🚀 Application URLs
```bash
✅ Swagger UI (recommended):  http://localhost:5246/swagger
✅ HTTP API:                  http://localhost:5246/api
✅ HTTPS API:                 https://localhost:7181/api
```

### 🔍 Quick Health Checks
```bash
# API Health (should return customers)
curl http://localhost:5246/api/customers

# Database Health (should return products)
curl http://localhost:5246/api/products

# Full API Test
curl http://localhost:5246/api/orders
```

---

## 🛠️ Troubleshooting

### ❌ SQL Server Issues

**Problem: Login failed for user 'sa'**
```bash
Error: Login failed for user 'sa'
```
**Solutions:**
```bash
# 1. Check password in appsettings.json = docker-compose.yml
#    Both must have: "TestSenVang@Password"

# 2. Reset SQL Server container
docker-compose down
docker volume rm senvangtest_sqlserver_data
docker-compose up -d

# 3. Wait 15-20 seconds for SQL Server initialization
```

**Problem: Database connection timeout**
```bash
# Check SQL Server is ready
docker logs sqlserver_ordermanagement | grep "SQL Server is now ready"

# Manual connection test
docker exec -it sqlserver_ordermanagement /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "TestSenVang@Password" -Q "SELECT @@VERSION"
```

### ❌ Port Conflicts

**Problem: Port 5246 already in use**
```bash
# Find process using port 5246
netstat -ano | findstr :5246

# Kill process
taskkill /PID <process-id> /F

# Or change port in launchSettings.json
```

**Problem: Port 1433 (SQL Server) already in use**
```bash
# Check if local SQL Server is running
Get-Service MSSQLSERVER

# Stop local SQL Server if needed
Stop-Service MSSQLSERVER

# Or change port in docker-compose.yml
```

### ❌ Redis Issues

**Problem: Redis connection failed**
```bash
# Check Redis container
docker logs redis_ordermanagement

# Test Redis manually
docker exec -it redis_ordermanagement redis-cli ping

# Redis restart
docker restart redis_ordermanagement
```

### ❌ Docker Issues

**Problem: Docker containers won't start**
```bash
# Check Docker Desktop is running
docker info

# Reset all containers
docker-compose down
docker-compose up -d --force-recreate

# Clean Docker system
docker system prune -a
```

**Problem: Volume permission issues**
```bash
# Reset Docker volumes
docker-compose down
docker volume prune
docker-compose up -d
```

### ❌ .NET Issues

**Problem: Package restore failed**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Build solution
dotnet build
```

**Problem: Build errors**
```bash
# Clean build
dotnet clean
dotnet build

# Rebuild specific project
dotnet build src/OrderManagement.API
```

---

## 🔄 Development Workflow

### 🚀 Daily Development
```bash
# 1. Start containers (chỉ cần 1 lần)
docker-compose up -d

# 2. Run với watch mode
cd src/OrderManagement.API
dotnet watch run

# 3. Code changes will auto-reload
```

### 🧪 Testing Workflow  
```bash
# Run unit tests
dotnet test

# Run specific test
dotnet test --filter "CustomerServiceTests"

# Test with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 📦 Adding New Features
```bash
# 1. Add package to specific project
dotnet add src/OrderManagement.Infrastructure package NewPackage

# 2. Update database (if needed)
# Note: We use EnsureCreated(), not migrations

# 3. Run tests
dotnet test

# 4. Test API endpoints in Swagger
```

---

## 🔧 Environment Configuration

### 📁 appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderManagementDB;User Id=sa;Password=TestSenVang@Password;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  }
}
```

### 🐳 docker-compose.yml key settings
```yaml
environment:
  SA_PASSWORD: "TestSenVang@Password"  # Must match appsettings.json
  ACCEPT_EULA: "Y"
  MSSQL_PID: "Express"
```

### 🚀 Port Configuration
Change ports in `src/OrderManagement.API/Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7181;http://localhost:5246"
}
```

---

## 📞 Support & Next Steps

### ✅ Setup Success Checklist
1. ✅ Docker containers running: `docker ps`
2. ✅ API responds: `curl http://localhost:5246/api/customers`
3. ✅ Swagger UI loads: http://localhost:5246/swagger
4. ✅ Database has seed data: 3 customers, 4 products, 3 orders
5. ✅ Redis working: No cache errors in logs

### 📚 Next Steps
- 📋 Read [API Documentation](API.md) for endpoint details
- 🏗️ Check [Architecture](ARCHITECTURE.md) for system design  
- 🧪 Import [Postman Collection](../postman_collection.json)
- 🔧 See [Configuration](CONFIG.md) for advanced settings

### 🆘 Still Having Issues?
1. Check [Docker troubleshooting](#-docker-issues)
2. Verify [Prerequisites](#-prerequisites) are installed
3. Review [Port conflicts](#-port-conflicts) section
4. Try [clean start](#-docker-commands) with volume reset

**🚀 Happy Development!** 