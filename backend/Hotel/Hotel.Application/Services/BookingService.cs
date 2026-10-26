using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Hotel.Application.Services
{
    public class BookingService:IClientBookingService,IManagerBookingService
    {
        IBookingRepository _bookingRepo;
        IClientRepository _clientRepo;
        IRoomRepository _roomRepo;
        public BookingService(IBookingRepository bookingRepo, IClientRepository clientRepo,IRoomRepository roomRepo)
        {
            _bookingRepo = bookingRepo;
            _clientRepo = clientRepo;
            _roomRepo = roomRepo;
        }
        public async Task<List<DateOnly>> GetOccupiedDatesByMonthAsync(Guid roomId, int year,int month)
        {
            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комнаты с id {roomId} не существует");
            DateOnly startDate = new DateOnly(year,month,1);
            DateOnly endDate = startDate.AddMonths(1).AddDays(-1);
            List<Booking> overlappingBooking = await _bookingRepo.GetOverlappingWithPeriodAsync(room.Id,startDate,endDate);
            return overlappingBooking.SelectMany(b => b.DateRange())
                    .Where(d => d >= startDate && d <= endDate)
                    .ToList();   
        }
        private async Task<bool> IsRoomAvailableForPeriodAsync(Guid roomId,DateOnly checkIn,DateOnly checkOut)
        {
            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комнаты с id {roomId} не существует");
            List<Booking> overlappingBookings = await _bookingRepo.GetOverlappingWithPeriodAsync(room.Id,checkIn,checkOut);
            if (overlappingBookings.Count == 0) return true;
            else return false;
        }

        public async Task CreateBookingAsync( Guid userId,Guid roomId,DateOnly checkIn,DateOnly checkOut)
        { 
            Room room = await _roomRepo.GetRoomById(roomId)??throw new ArgumentException($"Комнаты с id {roomId} не существует");
            Client client = await _clientRepo.GetClientByUserId(userId) ?? throw new ArgumentException($"Клиент не зарегистрирован");
            bool availabilityRoom = await IsRoomAvailableForPeriodAsync(room.Id, checkIn, checkOut);
            if (!availabilityRoom) throw new ArgumentException("Комната не доступна для этих дат");
            Booking book = new Booking(client.Id, room.Id, checkIn, checkOut, room.Price);
            await _bookingRepo.AddBook(book);
            await _bookingRepo.SaveChangesAsync();

        }
        private async Task<Client> CreateClientAsync(string phoneNumber, string firstName, string lastName, string? patronymic)
        {
            PhoneNumber phone = new PhoneNumber(phoneNumber);
            FullName name = new FullName(firstName, lastName, patronymic);
            Client client = new Client(phone, name);
            await _clientRepo.AddClient(client);

            await _clientRepo.SaveChangesAsync();
            return client;
        }
        private async Task<bool> IsClientCreatedAsync(string phoneNumber, string firstName, string lastName, string? patronymic)
        {
            PhoneNumber phone = new PhoneNumber(phoneNumber);
            FullName name = new FullName(firstName, lastName, patronymic);
            return await _clientRepo.GetByNameAndPhoneAsync(name, phone) !=null;
        }
        private async Task<Client> TakeClient(string phoneNumber, string firstName, string lastName, string? patronymic)
        {
            PhoneNumber phone = new PhoneNumber(phoneNumber);
            FullName name = new FullName(firstName, lastName, patronymic);
            Client client = await _clientRepo.GetByNameAndPhoneAsync(name, phone);
            return client;
        }
        public async Task CreateBookingWithoutAuthAsync(string phoneNumber, string firstName, string lastName, string? patronymic,
            Guid roomId,DateOnly checkIn,DateOnly checkOut)
        {
            Room room = await _roomRepo.GetRoomById(roomId)??throw new ArgumentException($"Комнаты с id {roomId} не существует");
            bool availabilityRoom = await IsRoomAvailableForPeriodAsync(room.Id, checkIn, checkOut);
            if (!availabilityRoom) throw new ArgumentException("Комната не доступна для этих дат");
            Client client = await IsClientCreatedAsync(phoneNumber,firstName,lastName,patronymic) ? 
                await TakeClient(phoneNumber,firstName,lastName,patronymic) : await CreateClientAsync(phoneNumber,firstName,lastName,patronymic);
            Booking book = new Booking(client.Id, room.Id, checkIn, checkOut, room.Price);
            await _bookingRepo.AddBook(book);
            await _bookingRepo.SaveChangesAsync();

        }
        public async Task<Booking> CreateBookingAsync(string phoneNumber, string firstName, string lastName, string? patronymic,
            Guid roomId, DateOnly checkIn, DateOnly checkOut)
        {

            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комнаты с id {roomId} не существует");
            bool availabilityRoom = await IsRoomAvailableForPeriodAsync(room.Id, checkIn, checkOut);
            if (!availabilityRoom) throw new ArgumentException("Комната не доступна для этих дат");
            Client client = await IsClientCreatedAsync(phoneNumber, firstName, lastName, patronymic) ?
                await TakeClient(phoneNumber, firstName, lastName, patronymic) : await CreateClientAsync(phoneNumber, firstName, lastName, patronymic);
            Booking book = new Booking(client.Id, room.Id, checkIn, checkOut, room.Price);
            await _bookingRepo.AddBook(book);
            await _bookingRepo.SaveChangesAsync();
            return book;    
        }
        public async Task<List<Booking>> GetActiveBookingsAsync()
        {
            return await _bookingRepo.GetActiveBookingAsync();
        }
        public async Task<List<Booking>> GetActiveBookingForDateAsync(DateOnly date)
        {
            return await _bookingRepo.GetActiveBookingForDateAsync(date);
        }
        public async Task<List<Booking>> GetActiveAndCancelledForDateAsync(DateOnly date)
        {
            return await _bookingRepo.GetActiveAndCancelledForDateAsync(date);
        }
        public async Task<List<Booking>> GetActiveBookingsByRoomAsync(Guid roomId)
        {
            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комнаты с id {roomId} не существует");
            return await _bookingRepo.GetActiveBookingsByRoomAsync(room.Id);
        }
        public async Task<List<Booking>> GetTodaysActiveAndCancelledAsync()
        {
            return await _bookingRepo.GetTodaysActiveAndCancelledAsync();
        }
        public async Task<List<Booking>> GetTodaysActiveBookingAsync()
        {
            return await _bookingRepo.GetTodaysActiveBookingAsync();
        }
        public async Task<Booking> GetBookingByIdAsync(Guid id)
        {
            Booking book = await _bookingRepo.GetBookingByIdAsync(id) ?? throw new ArgumentException($"Брони с id {id} не существует");
            return book;
        }
        public async Task<List<Booking>> GetRentedBookingAsync()
        {
            return await _bookingRepo.GetRentedBookingsAsync();
        }
        public async Task CancelBookAsync(Guid bookId, string reason)
        {
            Booking book = await _bookingRepo.GetBookingByIdAsync(bookId) ?? throw new ArgumentException($"Брони с id {bookId} не существует");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Причина отмены не может быть пуста");
            book.CancelBook(reason);
            await _bookingRepo.SaveChangesAsync();
        }
        public async Task ResheduleBookingAsync(Guid bookId, DateOnly dateCheckIn)
        {
            Booking book = await _bookingRepo.GetBookingByIdAsync(bookId) ?? throw new ArgumentException($"Брони с id {bookId} не существует");
            if (book.Status != BookStatus.Book && book.Status != BookStatus.Rented) throw new ArgumentException($"Бронь должна быть активна!");
            book.ResheduleBooking(dateCheckIn);
            await _bookingRepo.SaveChangesAsync();
        }
        public async Task ExtendBookingAsync(Guid bookId, DateOnly dateCheckOut)
        {
            Booking book = await _bookingRepo.GetBookingByIdAsync(bookId) ?? throw new ArgumentException($"Брони с id {bookId} не существует");
            if (book.Status != BookStatus.Book && book.Status != BookStatus.Rented) throw new ArgumentException($"Бронь должна быть активна!");
            bool checkAvailable = await IsRoomAvailableForPeriodAsync(book.RoomId, book.DateCheckIn, dateCheckOut);
            if (!checkAvailable) throw new ArgumentException($"Бронь за этот период невозможна!");
            book.ExtendBooking(dateCheckOut);
            await _bookingRepo.SaveChangesAsync();
        }

    }
}
