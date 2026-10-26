using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Models;
using Hotel.Domain.Enums;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Hotel.Domain.Abstractions.Repositories;

namespace Hotel.DataAccess.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelDbContext _context;
        public BookingRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Booking>> GetActiveBookingAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Book)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetRentedBookingsAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Rented)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetActiveRentedBookingsAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Rented )
                .ToListAsync();
        }
        public async Task<List<Booking>> GetTodaysActiveAndCancelledAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => (b.Status==BookStatus.Book||b.Status==BookStatus.Canceled)&& b.DateCheckIn == today)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetTodaysActiveBookingAsync()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Book && b.DateCheckIn == today)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetActiveBookingForDateAsync(DateOnly date)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Book && b.DateCheckIn == date)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetActiveAndCancelledForDateAsync(DateOnly date)
        {
            return await _context.Bookings
               .AsNoTracking()
               .Where(b => b.Status != BookStatus.Ended && b.DateCheckIn == date)
               .OrderBy(b => b.DateCheckIn)
               .ToListAsync();
        }
        
        public async Task<List<Booking>> GetActiveByClientByPageAsync(Guid clientId, int page, int pageSize)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Book && b.ClientId == clientId)
                .OrderBy(b => b.DateCheckIn)
                .Skip((page - 1) * pageSize)  
                .Take(pageSize)                
                .ToListAsync();
        }

        public async Task<List<Booking>> GetCancelledByClientByPageAsync(Guid clientId, int page, int pageSize)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Canceled
                         && b.ReasonCancel != "Клиент уехал раньше"
                         && b.ClientId == clientId)
                .OrderBy(b => b.DateCheckIn)
                .Skip((page - 1) * pageSize) 
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetForRentHistorytByPageAsync(Guid clientId, int page, int pageSize)
        {
            int ourPage = 1;
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status != BookStatus.Book && b.ClientId == clientId
                && (b.ReasonCancel == "Клиент уехал раньше" || b.ReasonCancel == null))
                .Skip((page - ourPage) * pageSize)
                .Take(pageSize)
                .OrderByDescending(b => b.DateCheckOut)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetBookingsForRentsAsync(List<RentRoom> rents)
        {
            var bookingIds = rents.Select(r => r.BookId).ToList();

            return await _context.Bookings
                .AsNoTracking()
                .Where(b => bookingIds.Contains(b.Id))
                .OrderByDescending(b => b.DateCheckOut)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetActiveBookingsByRoomAsync(Guid roomId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == BookStatus.Book && b.RoomId == roomId)
                .ToListAsync();
        }
        public async Task<List<Booking>> GetOverlappingWithPeriodAsync(Guid roomId, DateOnly startDate, DateOnly endDate)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => (b.Status == BookStatus.Book || b.Status == BookStatus.Rented) && b.RoomId == roomId
                && (b.DateCheckIn <= endDate && b.DateCheckOut >= startDate))
                .ToListAsync();
        }
        public async Task<Booking?> GetBookingByIdAsync(Guid id)
        {
            return await _context.Bookings
                .FindAsync(id);
        }
        public async Task AddBook(Booking book)
        {
            await _context.Bookings.AddAsync(book);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
