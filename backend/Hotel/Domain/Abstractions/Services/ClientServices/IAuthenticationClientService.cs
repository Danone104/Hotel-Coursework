using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ClientServices
{
    public interface IAuthenticationClientService: IAuthenticationService
    {
        Task Register(string userName, string password, string phone, string firstName, string lastName, string? patronymic);
        Task<bool> IsNameAvailable(string userName);
    }
}
