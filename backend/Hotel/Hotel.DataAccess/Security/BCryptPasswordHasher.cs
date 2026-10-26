using System;
using System.Collections.Generic;
using System.Text;
using Hotel.Domain.Abstractions.Security;
using BCrypt.Net;

namespace Hotel.DataAccess.Security
{
    public class BCryptPasswordHasher :IPasswordHasher
    {
        public async Task<string> Hash(string password)
        {
            
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public async Task<bool> VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        } 
    }
}
