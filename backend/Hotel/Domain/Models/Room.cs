using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hotel.Domain.Models
{
    public class Room
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int CountPerson { get; private set; }
        public decimal Price { get; private set; }
        public RoomStatus Status { get; private set; } 
        private Room() { }

        public Room(string title,int count,string description,decimal price)
        {
            Id = Guid.NewGuid();
            if (title == null) throw new ArgumentException("Название номера обязательно");
            Title = title;
            if (count <=0) throw new ArgumentException("Количество людей номера должны быть положительными");
            CountPerson = count;
            if (price <=0) throw new ArgumentException("Цена должна быть положительной");
            Price = price;
            Status = RoomStatus.Ready;
            if (description == null) throw new ArgumentException("Описание номера обязательно");
            Description = description;
        }
        public void UpdateRoomInfo(string title,string description,decimal price,RoomStatus status,int count)
        {
            if (title == null) throw new ArgumentException("Название номера обязательно");
            Title = title;
            if (count <= 0) throw new ArgumentException("Количество людей номера должны быть положительными");
            CountPerson = count;
            if (price <= 0) throw new ArgumentException("Цена должна быть положительной");
            Price = price;
            if (status == RoomStatus.Deleted) throw new ArgumentException("Невозможно обновить статус на удаленный");
            Status = RoomStatus.Ready;
            if (description == null) throw new ArgumentException("Описание номера обязательно");
            Description = description;
        }
        public void DeleteRoom()
        {
            if (Status == RoomStatus.Deleted) throw new ArgumentException("Невозможно удалить удаленный номер");
            Status = RoomStatus.Deleted;
        }
    }
}
