using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Hotel.Domain.Enums;
using Hotel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Application.Services
{
    public class RoomService: IAdminRoomService,IManagerRoomService,IClientRoomService
    {
        IRoomRepository _roomRepo;
        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;   
        }
        public async Task<Room> GetRoomById(Guid id)
        {
            return await _roomRepo.GetRoomById(id)??throw new ArgumentException($"Комнаты с id {id} не существует");
        }
        public async Task<List<Room>> GetRoomsAsync()
        {
            return await _roomRepo.GetRooms();
        }
        public async Task<List<Room>> GetRoomsByPageAsync(int page, int pageSize)
        {
            return await _roomRepo.GetRoomsByPage(page, pageSize);
        }
        public async Task<List<Room>> GetRoomsForBookingsAsync(List<Booking> bookings)
        {
            List<Room> rooms =await _roomRepo.GetRoomsByBookingsAsync(bookings);
            return rooms;
        }
        public async Task<int> GetCountRooms()
        {
            return await _roomRepo.GetCountRooms();
        }
        public async Task<Guid> CreateRoom(string title, int count, string description, decimal price)
        {
            Room room = new Room(title, count, description, price);
            await _roomRepo.AddRoom(room);
            await _roomRepo.SaveChangesAsync();
            return room.Id;
        }
        public async Task UpdateRoom(Guid roomId, string title, string description, decimal price, RoomStatus status, int count)
        {
            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комната с id {roomId} не существует");
            room.UpdateRoomInfo(title, description, price, status, count);
            await _roomRepo.SaveChangesAsync();
        }
        public async Task DeleteRoom(Guid roomId)
        {
            Room room = await _roomRepo.GetRoomById(roomId) ?? throw new ArgumentException($"Комната с id {roomId} не существует");
            room.DeleteRoom();
            await _roomRepo.SaveChangesAsync();
        }
    }
}
