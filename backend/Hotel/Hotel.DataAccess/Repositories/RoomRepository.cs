using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _context;
        public RoomRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<List<Room>> GetRooms()
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => r.Status != RoomStatus.Deleted)
                .ToListAsync();
        }
        public async Task<List<Room>> GetRoomsByPage(int page, int pageSize)
        {
            int ourPage = 1;
            return await _context.Rooms
                .AsNoTracking()
                .Where(r=> r.Status == RoomStatus.Ready)
                .Skip((page - ourPage) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Room?> GetRoomById(Guid id)
        {
            return await _context.Rooms
                .FindAsync(id);
        }
        public async Task<List<Room>> GetRoomsByBookingsAsync(List<Booking> bookings)
        {
            var bookingIds = bookings.Select(b => b.RoomId).ToList();
            var rooms = await _context.Rooms
                .AsNoTracking()
                .Where(r => bookingIds.Contains(r.Id))
                .ToListAsync();

            return rooms;
        }

        public async Task<int> GetCountRooms()
        {
            return  await _context.Rooms.CountAsync();
        }
        public async Task AddRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
