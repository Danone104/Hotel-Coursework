using Hotel.Domain.Enums;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hotel.Domain.Models
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public FullName Name { get; private set; }
        public PhoneNumber TelephoneNum { get; private set; }
        private Employee(){}
        public Employee(Guid userId,FullName name, PhoneNumber number)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            if(name == null) throw new ArgumentNullException("Имя обязательно");
            Name = name;
            if (number == null) throw new ArgumentNullException("Номер телефона обязателен");
            TelephoneNum = number;
        }
        public void UpdateEmployeeInfo(FullName name,PhoneNumber number)
        {
            if (name == null) throw new ArgumentNullException("Имя обязательно");
            Name = name;
            if (number == null) throw new ArgumentNullException("Номер телефона обязателен");
            TelephoneNum = number;
        }
    }
} 
