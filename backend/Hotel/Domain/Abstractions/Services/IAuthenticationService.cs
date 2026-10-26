using Hotel.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services
{
    public interface IAuthenticationService
    {
        Task<string> Login(string userName, string password);
    }
}
