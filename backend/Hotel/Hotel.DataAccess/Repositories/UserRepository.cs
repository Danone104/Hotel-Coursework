using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Models;
using Hotel.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hotel.Domain.Abstractions.Repositories;

namespace Hotel.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HotelDbContext _context;
        public UserRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<bool> IsSystemHaveAdminsAsync()
        {
            return _context.Users
                .Where(c=>c.Role==Roles.Admin)
                .Count() > 0;
        }
        public async Task<List<User>> GetUsersEmployeesAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role != Roles.Client)
                .ToListAsync();
        }
        public async Task<User?> GetUserByUserName(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users
                .FindAsync(id);
        }
        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
