using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Models
{
    public class User
    {
        public Guid Id { get; private set; }
        public string PasswordHash { get; private set; }
        public string UserName { get; private set; }
        public Roles Role { get; private set; }
        private User() { }
        public User(string hashedPass, string name,Roles role)
        {
            Id= Guid.NewGuid();
            if (string.IsNullOrEmpty(hashedPass)) throw new ArgumentException("Пароль не может быть пустым");
            PasswordHash = hashedPass;
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Имя не может быть пустым");
            UserName = name;
            Role = role;
        }
        public void UpdateUserName(string name)
        { 
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Новое имя пользователя не может быть пустым");
            UserName = name;
        }
        public void UpdatePassword(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash)) throw new ArgumentException("Новый пароль не может быть пустым"); 
            PasswordHash = passwordHash;
        }

    }
}
