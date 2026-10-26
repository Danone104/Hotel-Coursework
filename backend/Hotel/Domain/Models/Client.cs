using Hotel.Domain.Enums;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hotel.Domain.Models
{
    public class Client
    {
        public Guid Id { get; private set; }
        public Guid? UserId {  get; private set; }
        public PhoneNumber TelephoneNum { get; private set; }
        public FullName Name { get; private set; }
        private Client() { }  
        public Client(PhoneNumber num, FullName name)
        {
            Id = Guid.NewGuid();
            if(num == null) throw new ArgumentNullException("Номер телефона обязателен");
            TelephoneNum = num;
            if (name == null) throw new ArgumentNullException("Имя клиента обязательно");
            Name = name;
        }
        public Client(Guid userId, PhoneNumber num,FullName name)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            if (num == null) throw new ArgumentNullException("Номер телефона обязателен");
            TelephoneNum = num;
            if (name == null) throw new ArgumentNullException("Имя клиента обязательно");
            Name = name;
        }
        public void LinkToUser(Guid userId)
        {
            if (UserId != null) throw new ArgumentException("Клиент уже зарегистрирован");
            UserId = userId;
        }

    }
}
