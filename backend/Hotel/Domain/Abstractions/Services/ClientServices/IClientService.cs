using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Domain.Abstractions.Services.ClientServices
{
    public interface IClientService
    {
        Task<Client> GetClientByUserIdAsync(Guid id);
    }
}
