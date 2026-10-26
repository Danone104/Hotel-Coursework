using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class ClientService: IManagerClientService, IClientService
    {
        IClientRepository _clientRepository;
        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public async Task<Client> GetClientByIdAsync(Guid id)
        {
            return await _clientRepository.GetClientById(id)?? throw new ArgumentException($"Клиента с id {id} не существует");
        }
        public async Task<Client> GetClientByUserIdAsync(Guid id)
        {
            return await _clientRepository.GetClientByUserId(id) ?? throw new ArgumentException($"Пользователь с id {id} не зарегистрирован");
        }
        public async Task<List<Client>> GetClientsForBookingsAsync(List<Booking> bookings)
        {
            return await _clientRepository.GetClientsByBookingsAsync(bookings);
        }
    }
}
