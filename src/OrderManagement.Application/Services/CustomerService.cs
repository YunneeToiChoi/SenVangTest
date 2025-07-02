using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private const string CUSTOMER_CACHE_KEY = "customer:";
    private const string CUSTOMERS_CACHE_KEY = "customers:all";

    public CustomerService(
        ICustomerRepository customerRepository,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var cachedCustomers = await _cacheService.GetAsync<IEnumerable<CustomerDto>>(CUSTOMERS_CACHE_KEY);
        if (cachedCustomers != null)
            return cachedCustomers;

        var customers = await _customerRepository.GetAllAsync();
        var customerDtos = customers.Select(MapToDto).ToList();

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(CUSTOMERS_CACHE_KEY, customerDtos, TimeSpan.FromMinutes(5));

        return customerDtos;
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var cacheKey = $"{CUSTOMER_CACHE_KEY}{id}";
        var cachedCustomer = await _cacheService.GetAsync<CustomerDto>(cacheKey);
        if (cachedCustomer != null)
            return cachedCustomer;

        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return null;

        var customerDto = MapToDto(customer);

        // Cache the result for 10 minutes
        await _cacheService.SetAsync(cacheKey, customerDto, TimeSpan.FromMinutes(10));

        return customerDto;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
    {
        var customer = new Customer
        {
            FullName = createCustomerDto.FullName,
            Address = createCustomerDto.Address,
            PhoneNumber = createCustomerDto.PhoneNumber
        };

        var createdCustomer = await _customerRepository.AddAsync(customer);
        var customerDto = MapToDto(createdCustomer);

        // Clear customers cache
        await _cacheService.RemoveAsync(CUSTOMERS_CACHE_KEY);

        return customerDto;
    }

    public async Task<CustomerDto> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            throw new ArgumentException($"Customer with ID {id} not found.");

        customer.FullName = updateCustomerDto.FullName;
        customer.Address = updateCustomerDto.Address;
        customer.PhoneNumber = updateCustomerDto.PhoneNumber;

        var updatedCustomer = await _customerRepository.UpdateAsync(customer);
        var customerDto = MapToDto(updatedCustomer);

        // Clear cache
        await _cacheService.RemoveAsync($"{CUSTOMER_CACHE_KEY}{id}");
        await _cacheService.RemoveAsync(CUSTOMERS_CACHE_KEY);

        return customerDto;
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var exists = await _customerRepository.ExistsAsync(id);
        if (!exists)
            throw new ArgumentException($"Customer with ID {id} not found.");

        await _customerRepository.DeleteAsync(id);

        // Clear cache
        await _cacheService.RemoveAsync($"{CUSTOMER_CACHE_KEY}{id}");
        await _cacheService.RemoveAsync(CUSTOMERS_CACHE_KEY);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            Address = customer.Address,
            PhoneNumber = customer.PhoneNumber
        };
    }
} 