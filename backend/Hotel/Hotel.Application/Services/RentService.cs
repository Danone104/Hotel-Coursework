using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Models;
using Hotel.Domain.Enums;
using Microsoft.Extensions.Logging;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class RentService : IManagerRentService
    {
        public IBookingRepository _bookRepo;
        public IRentRepository _rentRepo;
        public IClientRepository _clientRepo;
        public IRoomRepository _roomRepo;
        public RentService(IBookingRepository bookRepo, IRentRepository rentRepo, IClientRepository clientRepo,
            IRoomRepository roomRepo)
        {
            _bookRepo = bookRepo;
            _rentRepo = rentRepo;
            _clientRepo = clientRepo;
            _roomRepo = roomRepo;
        }
        public async Task<List<RentRoom>> GetActiveRentsAsync()
        {
            return await _rentRepo.GetActiveRentAsync();
        }
        public async Task AddNoteAsync(Guid rentId, string note)
        {
            RentRoom rent = await _rentRepo.GetRentByIdAsync(rentId) ?? throw new ArgumentException($"Аренды с id {rentId} не существует");
            rent.AddNote(note);
        }
        public async Task FixCheckout(Guid rentId)
        {
            RentRoom rent = await _rentRepo.GetRentByIdAsync(rentId) ?? throw new ArgumentException($"Аренды с id {rentId} не существует");
            Booking book = await _bookRepo.GetBookingByIdAsync(rent.BookId);
            Room room = await _roomRepo.GetRoomById(book.RoomId);
            rent.FixCheckOut(room.Price);
            await _rentRepo.SaveChangesAsync();
        }
        public async Task RentRoom(Guid bookId)
        {
            Booking book = await _bookRepo.GetBookingByIdAsync(bookId) ?? throw new ArgumentException($"Брони с id {bookId} не существует");
            RentRoom rent = new RentRoom(bookId);
            book.BookToRent();
            await _rentRepo.AddRentAsync(rent);
            await _rentRepo.SaveChangesAsync();
            await _bookRepo.SaveChangesAsync();

        }
    }
}
