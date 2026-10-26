using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Models
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid ClientId{ get; private set; }
        public Guid RoomId { get; private set; }  
        public DateOnly DateCheckIn { get; private set; }
        public DateOnly DateCheckOut { get; private set; }
        public decimal Price { get; private set; }
        public BookStatus Status { get; private set; }
        public string? ReasonCancel { get; private set; }
        private decimal _pricePerNight;
        private Booking() {}

        public Booking(Guid clientId,Guid roomId,DateOnly checkin, DateOnly checkout,decimal priceRoom)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
            RoomId= roomId;
            if (checkin < DateOnly.FromDateTime(DateTime.UtcNow)) throw new ArgumentException("Дата заселения не может быть меньше сегодняшней даты");
            if (checkin > checkout) throw new ArgumentException("Дата заселения не может быть больше даты выселения");
            DateCheckIn = checkin;
            DateCheckOut = checkout;
            if (priceRoom <= 0) throw new ArgumentException("Цена должна быть положительна");
            _pricePerNight = priceRoom;
            Price = CalculatePrice(checkin,checkout);
            Status = BookStatus.Book;
        }
        private decimal CalculatePrice(DateOnly checkin,DateOnly checkout)
        {
            return (checkout.DayNumber - checkin.DayNumber) * _pricePerNight;
        }

        public void CancelBook(string reasonCancel)
        {
            if (Status != BookStatus.Book && Status != BookStatus.Rented) throw new ArgumentException("Нельзя отменить бронирование, которое завершено или отменено");
            Status = BookStatus.Canceled;
            ReasonCancel = reasonCancel;
        }
        public void CancelBookByClient()
        {
            if (Status != BookStatus.Book) throw new ArgumentException("Нельзя отменить бронирование, которое завершено или отменено, или арендовано");
            Status = BookStatus.Canceled;
            ReasonCancel = "Отменено клиентом";
        }
        public void EndedBook()
        {
            if(Status != BookStatus.Book) throw new ArgumentException("Нельзя завершить бронирование, если оно отменено или завершено");
            Status = BookStatus.Ended;
        }
        public void ExtendBooking(DateOnly dateCheckOut)
        {
            if (dateCheckOut < DateCheckOut) throw new ArgumentException("Дата продления не должна быть меньше предыдущей даты");
            DateCheckOut = dateCheckOut;
            Price =CalculatePrice(DateCheckIn,DateCheckOut);
        }
        public void ResheduleBooking(DateOnly dateCheckIn)
        {
            if (dateCheckIn < DateCheckIn) throw new ArgumentException("Новая дата заселения не должна быть меньше предыдущей");
            if (dateCheckIn >= DateCheckOut) throw new ArgumentException("Дата заселения должна быть меньше даты выселения");
            DateCheckOut = dateCheckIn;
            CalculatePrice(DateCheckIn, DateCheckOut);
        }
        public void BookToRent()
        {
            if (Status != BookStatus.Book) throw new ArgumentException("Не забронировано!");
            Status= BookStatus.Rented;
        }
        public void EarlyCheckout()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today < DateCheckIn) throw new ArgumentException("Нельзя выселиться раньше заселения");
            DateCheckOut= today;
            CalculatePrice(DateCheckIn,DateCheckOut);
            Status = BookStatus.Canceled;
            ReasonCancel = "Клиент уехал раньше";
        }
        public List<DateOnly> DateRange()
        {
            DateOnly start = DateCheckIn;
            DateOnly end = DateCheckOut;
            List<DateOnly> dateRange =new List<DateOnly>();
            while (!(start > end))
            {
                dateRange.Add(start);
                start = start.AddDays(1);
            }
            return dateRange;



        } 

    }
}
