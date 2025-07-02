# Test script để verify các thay đổi
Write-Host "🚀 Testing Order Management System..." -ForegroundColor Green

# Stop và restart containers
Write-Host "📦 Restarting Docker containers..." -ForegroundColor Yellow
docker-compose down
docker-compose up -d

# Wait for services to start
Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Build và run API
Write-Host "🔨 Building application..." -ForegroundColor Yellow
dotnet build

Write-Host "🌐 Starting API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-Command", "cd 'src/OrderManagement.API'; dotnet run" -WindowStyle Normal

# Wait for API to start
Write-Host "⏳ Waiting for API to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Test API endpoints
Write-Host "🧪 Testing API endpoints..." -ForegroundColor Green

# Test 1: Get customers (check seed data)
Write-Host "`n1. Testing GET /api/customers (seed data):" -ForegroundColor Cyan
try {
    $customers = Invoke-RestMethod -Uri "https://localhost:7092/api/customers" -SkipCertificateCheck
    Write-Host "✅ Found $($customers.Count) customers" -ForegroundColor Green
    $customers | ForEach-Object { Write-Host "   - $($_.fullName): $($_.phoneNumber)" }
} catch {
    Write-Host "❌ Error getting customers: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get orders (check seed data)
Write-Host "`n2. Testing GET /api/orders (seed data):" -ForegroundColor Cyan
try {
    $orders = Invoke-RestMethod -Uri "https://localhost:7092/api/orders" -SkipCertificateCheck
    Write-Host "✅ Found $($orders.Count) orders" -ForegroundColor Green
    $orders | ForEach-Object { Write-Host "   - Order $($_.orderId): $($_.customerName) - $($_.totalAmount) VND" }
} catch {
    Write-Host "❌ Error getting orders: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Try to create customer with duplicate phone
Write-Host "`n3. Testing phone number uniqueness:" -ForegroundColor Cyan
$duplicateCustomer = @{
    fullName = "Test Duplicate"
    address = "Test Address"
    phoneNumber = "0901234567"  # Same as existing customer
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Uri "https://localhost:7092/api/customers" -Method POST -Body $duplicateCustomer -ContentType "application/json" -SkipCertificateCheck
    Write-Host "❌ Should have failed - unique constraint not working!" -ForegroundColor Red
} catch {
    Write-Host "✅ Unique constraint working - duplicate phone rejected" -ForegroundColor Green
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test 4: Create customer with unique phone
Write-Host "`n4. Testing create customer with unique phone:" -ForegroundColor Cyan
$newCustomer = @{
    fullName = "Test Customer"
    address = "Test Address"
    phoneNumber = "0999888777"  # Unique phone
} | ConvertTo-Json

try {
    $result = Invoke-RestMethod -Uri "https://localhost:7092/api/customers" -Method POST -Body $newCustomer -ContentType "application/json" -SkipCertificateCheck
    Write-Host "✅ Customer created successfully with ID: $($result.customerId)" -ForegroundColor Green
} catch {
    Write-Host "❌ Error creating customer: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Test completed! Check Swagger UI: https://localhost:7092/swagger" -ForegroundColor Green
Write-Host "📊 Database should now have:" -ForegroundColor Yellow
Write-Host "   - 3 Customers (unique phone numbers)" -ForegroundColor White
Write-Host "   - 4 Products" -ForegroundColor White  
Write-Host "   - 3 Orders with OrderItems" -ForegroundColor White 