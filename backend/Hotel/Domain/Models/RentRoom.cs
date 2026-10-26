using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hotel.Domain.Models
{
    public class RentRoom
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public DateOnly DateCheckIn { get; private set; }
        public DateOnly? DateCheckOut{ get; private set; }
        public decimal? Price { get; private set; }
        public RentStatus Status { get; private set; }
        public string Note { get; private set; }
        
        private RentRoom() { }
        public RentRoom(Guid bookId)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            DateCheckIn = DateOnly.FromDateTime(DateTime.UtcNow);
            Status = RentStatus.Rented;
            Note = "";
        }
        public void AddNote(string note)
        {
            if (Status != RentStatus.Rented) throw new ArgumentException("Примечание можно добавлять только, если комната сдана этому человеку");
            if (note == null) throw new ArgumentNullException("Примечание не может быть пустым");
            Note = note;
        }
        public void FixCheckOut(decimal roomPrice)
        {
            if (Status != RentStatus.Rented) throw new ArgumentException("Нельзя завершить аренду, если комната не сдана");
            DateCheckOut = DateOnly.FromDateTime(DateTime.UtcNow);
            if (DateCheckIn == DateCheckOut)
            {
                Price = roomPrice/2;
            }
            else CalculatePrice(roomPrice);
            Status = RentStatus.Checking;
        }
        private void CalculatePrice(decimal roomPrice)
        {
            Price = (DateCheckOut.Value.DayNumber - DateCheckIn.DayNumber)*roomPrice;
            
        }
            
    }
}
