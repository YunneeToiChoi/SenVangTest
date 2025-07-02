using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ICacheService cacheService)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        var orders = await _orderRepository.GetByDateRangeAsync(fromDate, toDate);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var cacheKey = $"order:{id}";
        var cachedOrder = await _cacheService.GetAsync<OrderDto>(cacheKey);
        if (cachedOrder != null)
            return cachedOrder;

        var order = await _orderRepository.GetByIdWithItemsAsync(id);
        if (order == null)
            return null;

        var orderDto = MapToDto(order);
        await _cacheService.SetAsync(cacheKey, orderDto, TimeSpan.FromMinutes(5));
        return orderDto;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var customerExists = await _customerRepository.ExistsAsync(createOrderDto.CustomerId);
        if (!customerExists)
            throw new ArgumentException($"Customer with ID {createOrderDto.CustomerId} not found.");

        var productIds = createOrderDto.OrderItems.Select(oi => oi.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds);
        
        if (products.Count() != productIds.Count)
            throw new ArgumentException("One or more products not found.");

        var order = new Order
        {
            CustomerId = createOrderDto.CustomerId,
            OrderDate = DateTime.Now,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;
        foreach (var item in createOrderDto.OrderItems)
        {
            var product = products.First(p => p.ProductId == item.ProductId);
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.Quantity * orderItem.UnitPrice;
        }

        order.TotalAmount = totalAmount;

        var createdOrder = await _orderRepository.AddAsync(order);
        
        var customer = await _customerRepository.GetByIdAsync(createOrderDto.CustomerId);
        createdOrder.Customer = customer!;

        foreach (var item in createdOrder.OrderItems)
        {
            item.Product = products.First(p => p.ProductId == item.ProductId);
        }

        var orderDto = MapToDto(createdOrder);

        return orderDto;
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.FullName ?? "",
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
} 