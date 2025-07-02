using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? customerId)
    {
        IEnumerable<OrderDto> orders;

        if (customerId.HasValue)
        {
            orders = await _orderService.GetOrdersByCustomerIdAsync(customerId.Value);
        }
        else if (fromDate.HasValue && toDate.HasValue)
        {
            orders = await _orderService.GetOrdersByDateRangeAsync(fromDate.Value, toDate.Value);
        }
        else
        {
            orders = await _orderService.GetAllOrdersAsync();
        }

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound($"Order with ID {id} not found.");

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Lỗi khi tạo đơn hàng: {ex.Message}");
        }
    }
} 