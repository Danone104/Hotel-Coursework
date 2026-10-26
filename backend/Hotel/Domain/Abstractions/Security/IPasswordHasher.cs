using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Security
{
    public interface IPasswordHasher
    {
        Task<string> Hash(string password);
        Task<bool> VerifyPassword(string password,string HashedPassword);
    }
}
