using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using Hotel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class AuthenticationService : IAuthenticationService, IAuthenticationClientService
    {
        private IClientRepository _clientRepo;
        private IUserRepository _userRepo;
        private IPasswordHasher _passwordHasher;
        private IJwtProvider _jwtProvider;
        public AuthenticationService(IClientRepository clientRepo, IUserRepository userRepo,
            IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _clientRepo = clientRepo;
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Имя пользователя не может быть пустым");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Пароль не должен быть пустым");
            User user = await _userRepo.GetUserByUserName(userName) ?? throw new UnauthorizedAccessException("Пользователь с таким логином не найден");
            if (!await _passwordHasher.VerifyPassword(password, user.PasswordHash)) throw new UnauthorizedAccessException("Пароль введен неправильно");
            return _jwtProvider.GenerateToken(user);
        }
        public async Task Register(string userName, string password, string phone,
            string firstName, string lastName, string? patronymic)
        {
            if (!(await IsNameAvailable(userName))) throw new ArgumentException("Имя пользователя занято");
            string passwordHash = await _passwordHasher.Hash(password);
            User user = new User( passwordHash,userName, Roles.Client);
            PhoneNumber phoneNumber = new PhoneNumber(phone);
            FullName fullName = new FullName(firstName, lastName, patronymic);
            Client client = await _clientRepo.GetByNameAndPhoneAsync(fullName, phoneNumber);
            if (client == null)
            {
                client = new Client(phoneNumber, fullName);
                client.LinkToUser(user.Id);
                await _clientRepo.AddClient(client);
            }
            else client.LinkToUser(user.Id);
            await _userRepo.AddUser(user);
            await _userRepo.SaveChangesAsync();
            await _clientRepo.SaveChangesAsync();


        }
        public async Task<bool> IsNameAvailable(string userName)
        {
            return await _userRepo.GetUserByUserName(userName) == null;
        }


    }
}
