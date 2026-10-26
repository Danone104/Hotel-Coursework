    using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Hotel.Domain.ValueObjects
{
    public record class PhoneNumber
    {
        public string Value { get; }
        public PhoneNumber(string value)
        {
            if (!IsValid(value)) throw new ArgumentException("Неверный формат телефона");
            Value = value;
        }
        private bool IsValid(string Value)
        {
            if (!Regex.IsMatch(Value, @"^(\+7|8)\d{10}")) return false;
            return true;
        }
        
    
    }
}
