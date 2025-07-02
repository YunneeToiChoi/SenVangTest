using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound($"Customer with ID {id} not found.");

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createCustomerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
            return Ok(customer);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 