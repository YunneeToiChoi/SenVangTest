# 📋 API Documentation

## 🌐 Base URLs
```
HTTP:        http://localhost:5246/api
HTTPS:       https://localhost:7181/api
Swagger UI:  http://localhost:5246/swagger
```

---

## 👥 Customer Management

### GET /api/customers
Lấy danh sách tất cả khách hàng (Redis cached 10 min)
```http
GET http://localhost:5246/api/customers
```

### GET /api/customers/{id}
Lấy khách hàng theo ID
```http
GET http://localhost:5246/api/customers/1
```

### POST /api/customers
Tạo khách hàng mới (phone number UNIQUE)
```http
POST http://localhost:5246/api/customers
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "address": "123 ABC Street, TP.HCM", 
  "phoneNumber": "0901234567"
}
```

### PUT /api/customers/{id}
Cập nhật khách hàng
```http
PUT http://localhost:5246/api/customers/1
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A Updated",
  "address": "456 XYZ Street, TP.HCM",
  "phoneNumber": "0901111111"
}
```

### DELETE /api/customers/{id}
Xóa khách hàng
```http
DELETE http://localhost:5246/api/customers/1
```

---

## 📦 Product Management

### GET /api/products
Lấy danh sách tất cả sản phẩm (Redis cached 15 min)
```http
GET http://localhost:5246/api/products
```

### GET /api/products/{id}
Lấy sản phẩm theo ID
```http
GET http://localhost:5246/api/products/1
```

**Available Product IDs:** 1, 2, 3, 4 (from seed data)

---

## 📋 Order Management

### GET /api/orders
Lấy danh sách đơn hàng với filtering
```http
# All orders
GET http://localhost:5246/api/orders

# Filter by customer
GET http://localhost:5246/api/orders?customerId=1

# Filter by date range
GET http://localhost:5246/api/orders?fromDate=2024-12-01&toDate=2024-12-31

# Combined filters
GET http://localhost:5246/api/orders?customerId=1&fromDate=2024-12-01&toDate=2024-12-31
```

### GET /api/orders/{id}
Lấy chi tiết đơn hàng
```http
GET http://localhost:5246/api/orders/1
```

### POST /api/orders
Tạo đơn hàng mới (auto-calculate total)
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

**Auto-processing:**
- ✅ Auto-calculate TotalAmount
- ✅ Auto-set OrderDate = DateTime.Now
- ✅ Validate Customer & Products exist
- ✅ Get current product prices

---

## ✅ Validation Rules

### Customer
```
fullName:    Required, max 100 chars
address:     Required, max 200 chars  
phoneNumber: Required, max 15 chars, valid phone format, UNIQUE
```

### Order
```
customerId:  Required, must exist
orderItems:  Required, min 1 item
productId:   Required, must exist (each item)
quantity:    Required, min 1 (each item)
```

---

## 🔍 HTTP Status Codes

| Code | Description |
|------|-------------|
| 200  | OK - GET, PUT success |
| 201  | Created - POST success |
| 204  | No Content - DELETE success |
| 400  | Bad Request - Validation error |
| 404  | Not Found - Resource not found |

---

## 🧪 Quick Test

### Health Check
```bash
curl http://localhost:5246/api/customers
```

### Create Customer
```bash
curl -X POST http://localhost:5246/api/customers \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test User","address":"Test Address","phoneNumber":"0999888777"}'
```

### Create Order
```bash
curl -X POST http://localhost:5246/api/orders \
  -H "Content-Type: application/json" \
  -d '{"customerId":1,"orderItems":[{"productId":1,"quantity":1}]}'
```

---

## 📚 More Details

- 🚀 [Setup Guide](SETUP.md) - Installation & running
- 🏗️ [Architecture](ARCHITECTURE.md) - System design
- 🔧 [Configuration](CONFIG.md) - Settings & ports
- 🧪 [Testing](TESTING.md) - Unit tests & Postman
- 📁 [Postman Collection](../postman_collection.json) - Import for testing

**🎯 Use Swagger UI for interactive testing: http://localhost:5246/swagger** 