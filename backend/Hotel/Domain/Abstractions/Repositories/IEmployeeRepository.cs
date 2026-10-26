using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;

namespace Hotel.Domain.Abstractions.Repositories
{
    public interface IEmployeeRepository
    {
        Task AddEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeByIdAsync(Guid id);
        Task<Employee?> GetEmployeeByUserIdAsync(Guid userId);
        Task<List<Employee>> GetEmployeesAsync();
        Task<List<Employee>> GetEmployeesByNameAsync(FullName name);
        Task SaveChangesAsync();
    }
}