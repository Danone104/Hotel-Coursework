using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Models;
using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.ValueObjects;

namespace Hotel.Application.Services
{
    public class SystemInitializeService:ISystemInitializer
    {
        private IUserRepository _userRepo;
        private IEmployeeRepository _employeeRepo;
        private IPasswordHasher _passwordHasher;
        private const string DEFAULT_ADMIN_PASSWORD = "admin123";
        public SystemInitializeService(IUserRepository userRepo,IPasswordHasher passwordHasher,IEmployeeRepository employeeRepo)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _employeeRepo = employeeRepo;
        }
        public async Task<bool> IsSystemNeedInitializeAsync()
        {
            return await _userRepo.IsSystemHaveAdminsAsync();
        }
        public async Task InitializeSystemAsync()
        {
            if ((await _userRepo.IsSystemHaveAdminsAsync())) throw new ArgumentException("Система уже инициализирована");
            string passwordHash= await _passwordHasher.Hash(DEFAULT_ADMIN_PASSWORD);
            User admin = new User(passwordHash, "admin", Roles.Admin);
            await _userRepo.AddUser(admin);
            await _userRepo.SaveChangesAsync();
            await CreateEmployeForAdminAsync(admin);
        }
        private async Task CreateEmployeForAdminAsync(User admin)
        {
            FullName fullName = new FullName("Admin", "Admin","Admin");
            PhoneNumber phone =new PhoneNumber( "+77777777777");
            Employee employee = new Employee(admin.Id, fullName, phone);
            await _employeeRepo.AddEmployeeAsync(employee);
            await _employeeRepo.SaveChangesAsync();

        }
    }
}
