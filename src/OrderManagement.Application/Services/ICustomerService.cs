using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
    Task<CustomerDto> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);
    Task DeleteCustomerAsync(int id);
} 