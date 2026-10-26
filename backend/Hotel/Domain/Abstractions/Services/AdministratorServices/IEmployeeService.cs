using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.AdministratorServices
{
    public interface IEmployeeService
    {
        Task CreateEmployee(string userName, string password, string firstName, string lastName, string? patronymic, string phone, Roles role);
        Task<List<Employee>> GetEmployeesAsync();
        Task<List<User>> GetUsersEmployeesAsync();
        Task DeleteEmployee(Guid id);
        Task UpdateEmployee(Guid id, string userName, string password, string firstName, string lastName,
          string? patronymic, string phone, Roles role);
        Task<bool> IsNameAvailable(string userName);
    }
}
