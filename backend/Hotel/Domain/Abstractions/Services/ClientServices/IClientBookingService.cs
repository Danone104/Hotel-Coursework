using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ClientServices
{
    public interface IClientBookingService
    {
        Task<List<DateOnly>> GetOccupiedDatesByMonthAsync(Guid roomId, int year, int month);
        Task CreateBookingAsync(Guid userId, Guid roomId, DateOnly checkin, DateOnly checkOut);
        Task CreateBookingWithoutAuthAsync(string phoneNumber, string firstName, string lastName, string? patronymic,
            Guid roomId, DateOnly checkIn, DateOnly checkOut);

    }

}
