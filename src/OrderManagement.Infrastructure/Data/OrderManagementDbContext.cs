using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data;

public class OrderManagementDbContext : DbContext
{
    public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
            
            // Unique constraint for phone number
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, FullName = "Nguyễn Văn A", Address = "123 Đường ABC, TP.HCM", PhoneNumber = "0901234567" },
            new Customer { CustomerId = 2, FullName = "Trần Thị B", Address = "456 Đường XYZ, Hà Nội", PhoneNumber = "0987654321" },
            new Customer { CustomerId = 3, FullName = "Lê Văn C", Address = "789 Đường DEF, Đà Nẵng", PhoneNumber = "0912345678" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Laptop Dell", Price = 15000000 },
            new Product { ProductId = 2, Name = "Mouse Logitech", Price = 500000 },
            new Product { ProductId = 3, Name = "Keyboard Mechanical", Price = 1200000 },
            new Product { ProductId = 4, Name = "Monitor Samsung", Price = 8000000 }
        );

        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order 
            { 
                OrderId = 1, 
                CustomerId = 1, 
                OrderDate = new DateTime(2024, 12, 1, 10, 30, 0), 
                TotalAmount = 31000000 
            },
            new Order 
            { 
                OrderId = 2, 
                CustomerId = 2, 
                OrderDate = new DateTime(2024, 12, 2, 14, 15, 0), 
                TotalAmount = 9200000 
            },
            new Order 
            { 
                OrderId = 3, 
                CustomerId = 1, 
                OrderDate = new DateTime(2024, 12, 3, 9, 45, 0), 
                TotalAmount = 1700000 
            }
        );

        // Seed OrderItems
        modelBuilder.Entity<OrderItem>().HasData(
            // Order 1 items
            new OrderItem 
            { 
                OrderItemId = 1, 
                OrderId = 1, 
                ProductId = 1, 
                Quantity = 2, 
                UnitPrice = 15000000 
            },
            new OrderItem 
            { 
                OrderItemId = 2, 
                OrderId = 1, 
                ProductId = 2, 
                Quantity = 2, 
                UnitPrice = 500000 
            },
            
            // Order 2 items
            new OrderItem 
            { 
                OrderItemId = 3, 
                OrderId = 2, 
                ProductId = 4, 
                Quantity = 1, 
                UnitPrice = 8000000 
            },
            new OrderItem 
            { 
                OrderItemId = 4, 
                OrderId = 2, 
                ProductId = 3, 
                Quantity = 1, 
                UnitPrice = 1200000 
            },
            
            // Order 3 items
            new OrderItem 
            { 
                OrderItemId = 5, 
                OrderId = 3, 
                ProductId = 3, 
                Quantity = 1, 
                UnitPrice = 1200000 
            },
            new OrderItem 
            { 
                OrderItemId = 6, 
                OrderId = 3, 
                ProductId = 2, 
                Quantity = 1, 
                UnitPrice = 500000 
            }
        );
    }
} 