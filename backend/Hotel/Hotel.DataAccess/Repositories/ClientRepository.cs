using Hotel.DataAccess.DBAdapter;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using Hotel.Domain.Abstractions.Repositories;

namespace Hotel.DataAccess.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly HotelDbContext _context;
        public ClientRepository(HotelDbContext context)
        {
            _context = context;
        }
        public async Task<Client?> GetClientById(Guid id)
        {
            return await _context.Clients
                .FindAsync(id);

        }
        public async Task<Client?> GetByNameAndPhoneAsync(FullName name, PhoneNumber phone)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Name == name && c.TelephoneNum == phone);
        }
        public async Task<Client?> GetClientByUserId(Guid userId)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<List<Client>> GetClientsByBookingsAsync(List<Booking> bookings)
        {
            var clientIds = bookings.Select(b => b.ClientId).Distinct().ToList();

            return await _context.Clients
                .AsNoTracking()
                .Where(c => clientIds.Contains(c.Id))
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddClient(Client client)
        {
            await _context.Clients.AddAsync(client);
        }
    }
}
