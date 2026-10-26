using Hotel.DataAccess;
using Hotel.Domain.Abstractions;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class RentHistoryService : IClientRentHistoryService
    {
        private IRentRepository _rentRepo;
        private IClientRepository _clientRepo;
        private IBookingRepository _bookingRepo;
        public RentHistoryService(IRentRepository rentRepo, IClientRepository clientRepo, IBookingRepository bookingRepo)
        {
            _rentRepo = rentRepo;
            _bookingRepo = bookingRepo;
            _clientRepo = clientRepo;
        }
        public async Task<List<RentRoom>> GetRentsHistoryAsync(Guid userId, int page, int pageSize)
        {
            Client client = await _clientRepo.GetClientByUserId(userId) ?? throw new ArgumentException($"Клиент не зарегистрирован");
            List<Booking> noActiveBookings = await _bookingRepo.GetForRentHistorytByPageAsync(client.Id, page, pageSize);
            if (noActiveBookings.Count == 0) return new List<RentRoom>();
            return await _rentRepo.GetRentsByBookingsAsync(noActiveBookings);
        }
        public async Task<List<Booking>> GetRentsBookingsAsync(List<RentRoom> rents)
        {
            return await _bookingRepo.GetBookingsForRentsAsync(rents);
        }
        

        public async Task<List<Booking>> GetActiveBookingsAsync(Guid userId, int page, int pageSize)
        {
            Client client = await _clientRepo.GetClientByUserId(userId) ?? throw new ArgumentException($"Клиент не зарегистрирован");
            List<Booking> bookings = await _bookingRepo.GetActiveByClientByPageAsync(client.Id, page, pageSize);
            return bookings;
        }
        public async Task<List<Booking>> GetCancelledBookingsAsync(Guid userId, int page, int pageSize)
        {
            Client client = await _clientRepo.GetClientByUserId(userId) ?? throw new ArgumentException($"Клиент не зарегистрирован");
            return await _bookingRepo.GetCancelledByClientByPageAsync(client.Id, page, pageSize);
        }
        public async Task CancelActiveBooking(Guid userId, Guid bookId)
        {
            Booking booking = await _bookingRepo.GetBookingByIdAsync(bookId) ?? throw new ArgumentException($"Брони с id {bookId} не существует");
            Client client = await _clientRepo.GetClientByUserId(userId) ?? throw new ArgumentException($"Клиент не зарегистрирован");
            if (client.Id != booking.ClientId) throw new ArgumentException("Бронь не соответствует клиенту");
            booking.CancelBookByClient();
            await _bookingRepo.SaveChangesAsync();
        }
    }
}
