using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ManagerServices
{
    public interface IManagerBookingService
    {
        Task<List<Booking>> GetActiveBookingsAsync();
        Task<List<Booking>> GetActiveBookingForDateAsync(DateOnly date);
        Task<List<Booking>> GetActiveAndCancelledForDateAsync(DateOnly date);
        Task<List<Booking>> GetActiveBookingsByRoomAsync(Guid roomId);
        Task<List<Booking>> GetTodaysActiveAndCancelledAsync();
        Task<List<Booking>> GetTodaysActiveBookingAsync();
        Task<List<Booking>> GetRentedBookingAsync();
        Task<Booking> GetBookingByIdAsync(Guid id);
        Task CancelBookAsync(Guid bookId,string reason);
        Task ResheduleBookingAsync(Guid bookId,DateOnly dateCheckIn);
        Task ExtendBookingAsync(Guid bookId,DateOnly dateCheckOut);
        Task<List<DateOnly>> GetOccupiedDatesByMonthAsync(Guid roomId, int year, int month);
        Task<Booking> CreateBookingAsync(string phoneNumber, string firstName, string lastName, string? patronymic,
            Guid roomId, DateOnly checkIn, DateOnly checkOut);
    }
}
