using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
} 