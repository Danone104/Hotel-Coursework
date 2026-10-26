using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.ValueObjects
{
    public record class FullName
    {
        public string FirstName { get; }
        public string LastName{  get; }
        public string? Patronymic { get; }
        public FullName(string firstName, string lastName, string? patronymic)
        {
            if (!IsValidPartOFName(firstName)) throw new ArgumentException("Имя не может начинаться с маленькой буквы");
            FirstName = firstName;
            if(!IsValidPartOFName(lastName)) throw new ArgumentException("Фамилия не может начинаться с маленькой буквы");
            LastName = lastName;
            
            if (!(string.IsNullOrWhiteSpace(patronymic)))
            {
                if (!IsValidPartOFName(patronymic)) throw new ArgumentException("Отчество не может быть с маленькой");
                Patronymic = patronymic;
                
            }   
        }


        public override string ToString()
        {
            if(string.IsNullOrWhiteSpace(Patronymic)) return $"{FirstName} {LastName}";
            return $"{FirstName} {LastName} {Patronymic}";  
        }
        public bool IsValidPartOFName(string part)
        {
            if (part[0] == part.ToLower()[0]) return false;
            return true ;
        }
    }
}
