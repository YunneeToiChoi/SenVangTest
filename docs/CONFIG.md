# 🔧 Configuration Guide

## 📁 appsettings.json

### Full Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderManagementDB;User Id=sa;Password=TestSenVang@Password;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Connection String Breakdown
```bash
SQL Server Components:
- Server=localhost,1433     # SQL Server host & port
- Database=OrderManagementDB # Database name
- User Id=sa               # SQL Server username
- Password=TestSenVang@Password # Must match docker-compose.yml
- TrustServerCertificate=true  # For Docker SQL Server

Redis:
- localhost:6379           # Redis host & port (no auth)
```

---

## 🐳 docker-compose.yml

### Full Configuration
```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver_ordermanagement
    ports:
      - "1433:1433"
    environment:
      SA_PASSWORD: "TestSenVang@Password"  # Must match appsettings.json
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Express"
    volumes:
      - sqlserver_data:/var/opt/mssql
    restart: unless-stopped

  redis:
    image: redis:7-alpine
    container_name: redis_ordermanagement
    ports:
      - "6379:6379"
    restart: unless-stopped

volumes:
  sqlserver_data:
```

### Environment Variables
```bash
SQL Server:
- SA_PASSWORD: Master password (MUST be strong)
- ACCEPT_EULA: Accept End User License Agreement
- MSSQL_PID: Product ID (Express for free version)

Redis:
- No environment variables needed
- Default configuration is sufficient
```

---

## 🚀 Port Configuration

### Application Ports (launchSettings.json)
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5246"
    },
    "https": {
      "applicationUrl": "https://localhost:7181;http://localhost:5246"
    }
  }
}
```

### Infrastructure Ports
```bash
✅ SQL Server:   1433 (mapped to host)
✅ Redis:        6379 (mapped to host)
✅ API HTTP:     5246
✅ API HTTPS:    7181
```

### Change Ports (if needed)
```bash
# API Ports: Edit src/OrderManagement.API/Properties/launchSettings.json
# SQL Server: Edit docker-compose.yml ports section
# Redis: Edit docker-compose.yml ports section
```

---

## ⚙️ Redis Configuration

### Cache Settings (trong code)
```csharp
// Cache TTL Configuration
public static class CacheKeys
{
    public const int CustomerCacheTTL = 10; // 10 minutes
    public const int ProductCacheTTL = 15;  // 15 minutes  
    public const int OrderCacheTTL = 5;     // 5 minutes
}

// Cache Key Patterns
Customers: "customers:all", "customer:{id}"
Products:  "products:all", "product:{id}"
Orders:    "orders:all", "order:{id}"
```

### Redis Connection String Options
```bash
# Basic (current)
localhost:6379

# With authentication (if needed)
localhost:6379,password=yourpassword

# With SSL (production)
rediss://username:password@host:port

# Redis Cluster (production)
host1:6379,host2:6379,host3:6379
```

---

## 🔧 Entity Framework Configuration

### Database Context Setup
```csharp
// In Program.cs
services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

// Database ensures created (not migrations)
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
context.Database.EnsureCreated();
```

### Connection Pool Settings
```json
// In connection string (optional)
"DefaultConnection": "...;Max Pool Size=100;Min Pool Size=5;Connection Timeout=30;"
```

---

## 🔒 Security Configuration

### Password Requirements
```bash
SQL Server SA Password:
✅ Minimum 8 characters
✅ Must contain uppercase, lowercase, numbers
✅ Must contain special characters
✅ Current: "TestSenVang@Password"
```

### CORS Configuration
```csharp
// In Program.cs
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### HTTPS Configuration
```json
// Development certificate
dotnet dev-certs https --trust

// Production: Use proper SSL certificates
```

---

## 🧪 Environment-Specific Configs

### Development (current)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Production (future)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Production SQL Server connection",
    "Redis": "Production Redis connection"
  }
}
```

### Staging (future)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## 🔄 Configuration Validation

### Startup Checks
```csharp
// Database connection check
try
{
    context.Database.CanConnect();
    Console.WriteLine("✅ Database connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
}

// Redis connection check  
try
{
    await cacheService.GetAsync<string>("health-check");
    Console.WriteLine("✅ Redis connection successful");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Redis connection failed: {ex.Message}");
}
```

### Health Check Endpoints (future)
```csharp
// Add health checks
services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddRedis(redisConnectionString);

// Endpoint: /health
```

---

## 🛠️ Troubleshooting Configuration

### Common Issues

**1. Password Mismatch**
```bash
Error: Login failed for user 'sa'

Fix:
1. Check appsettings.json password
2. Check docker-compose.yml SA_PASSWORD
3. Both must be exactly: "TestSenVang@Password"
```

**2. Port Conflicts**
```bash
Error: Port already in use

Fix:
1. Check running processes: netstat -ano | findstr :5246
2. Kill process: taskkill /PID <pid> /F
3. Or change port in launchSettings.json
```

**3. Docker Not Running**
```bash
Error: Cannot connect to Docker daemon

Fix:
1. Start Docker Desktop
2. Check: docker info
3. Restart Docker if needed
```

---

## 📚 Configuration Best Practices

### ✅ Do's
- ✅ Use strong passwords
- ✅ Keep appsettings.json and docker-compose.yml in sync
- ✅ Use TrustServerCertificate=true for Docker SQL
- ✅ Set appropriate cache TTL values
- ✅ Use connection pooling

### ❌ Don'ts
- ❌ Don't commit production secrets
- ❌ Don't use weak passwords
- ❌ Don't expose database ports in production
- ❌ Don't disable SSL in production
- ❌ Don't use default Redis passwords

---

## 📚 Related Documentation

- 🚀 [Setup Guide](SETUP.md) - Step-by-step installation
- 📋 [API Documentation](API.md) - Endpoint details
- 🏗️ [Architecture](ARCHITECTURE.md) - System design
- 🧪 [Testing](TESTING.md) - Testing guides

**🔧 Configuration is key to successful deployment!** 