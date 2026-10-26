using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hotel.Domain.Abstractions.Repositories;

namespace Hotel.DataAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HotelDbContext _context;
        public EmployeeRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Employee>> GetEmployeesByNameAsync(FullName name)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.Name == name)
                .ToListAsync();
        }
        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
        {
            return await _context.Employees
                .FindAsync(id);
        }
        public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
        }
        public async Task DeleteEmployeeAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
