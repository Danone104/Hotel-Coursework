using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Hotel.Domain.Abstractions.Repositories;

namespace Hotel.DataAccess.Repositories
{
    public class RentRepository : IRentRepository
    {
        private readonly HotelDbContext _context;
        public RentRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<List<RentRoom>> GetActiveRentAsync()
        {
            return await _context.Rents
                .AsNoTracking()
                .Where(r => r.Status == RentStatus.Rented || r.Status == RentStatus.Checking)
                .ToListAsync();
        }
        public async Task<List<RentRoom>> GetRentsByBookingsAsync(List<Booking> bookings)
        {
            var bookingIds = bookings.Select(b => b.Id).ToList();

            return await _context.Rents
                .AsNoTracking()
                .Where(r => bookingIds.Contains(r.BookId))
                .ToListAsync();
        }
        public async Task<RentRoom?> GetRentByIdAsync(Guid id)
        {
            return await _context.Rents
                .FindAsync(id);
        }
        public async Task AddRentAsync(RentRoom rent)
        {
            await _context.Rents.AddAsync(rent);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
