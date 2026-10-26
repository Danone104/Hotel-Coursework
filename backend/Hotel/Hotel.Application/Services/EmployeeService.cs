using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository _employeeRepo;
        private IUserRepository _userRepo;
        private IPasswordHasher _passwordHasher;
        public EmployeeService(IEmployeeRepository employeeRepo, IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _employeeRepo.GetEmployeesAsync();
        }
        public async Task<List<User>> GetUsersEmployeesAsync()
        {
            return await _userRepo.GetUsersEmployeesAsync();
        }
        public async Task CreateEmployee(string userName, string password, string firstName, string lastName,
          string? patronymic, string phone, Roles role)
        {
            if (!(await IsNameAvailable(userName))) throw new ArgumentException("Пользователь с таким именем уже есть");
            User user = await CreateEmployeeUser(userName, password, role);
            PhoneNumber phoneNumber = new PhoneNumber(phone);
            FullName fullName = new FullName(firstName, lastName, patronymic);
            Employee employee = new Employee(user.Id, fullName, phoneNumber);
            await _employeeRepo.AddEmployeeAsync(employee);
            await _userRepo.SaveChangesAsync();

        }
        public async Task DeleteEmployee(Guid id)
        {
            Employee employee = await _employeeRepo.GetEmployeeByIdAsync(id)??throw new ArgumentException("Работник не найден");
            User user = await _userRepo.GetUserById(employee.UserId);
            await _userRepo.DeleteUserAsync(user); 
            await _userRepo.SaveChangesAsync();
            await _employeeRepo.SaveChangesAsync();
        }
        public async Task UpdateEmployee(Guid id, string userName, string password, string firstName, string lastName,
          string? patronymic, string phone, Roles role)
        {
            if (!(await IsNameAvailable(userName))) throw new ArgumentException("Пользователь с таким именем уже есть");
            Employee employee = await _employeeRepo.GetEmployeeByIdAsync(id)?? throw new ArgumentException("Работник не найден");
            User user = await _userRepo.GetUserById(employee.UserId);
            if(userName!= user.UserName) user.UpdateUserName(userName);
            if(!(await _passwordHasher.VerifyPassword(password,user.PasswordHash)))
            {
                string newPassHash= await _passwordHasher.Hash(password);
                user.UpdatePassword(password);

            }
            FullName fullName = new FullName(firstName, lastName, patronymic);
            PhoneNumber phoneNumber = new PhoneNumber(phone);
            employee.UpdateEmployeeInfo(fullName, phoneNumber);
            await _employeeRepo.UpdateEmployeeAsync(employee);
        }
        private async Task<User> CreateEmployeeUser(string userName, string password, Roles role)
        {
            string passHash = await _passwordHasher.Hash(password);
            User user = new User(passHash, userName, role);
            await _userRepo.AddUser(user);
            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<bool> IsNameAvailable(string userName)
        {
            return await _userRepo.GetUserByUserName(userName) == null;
        }
    }
}
