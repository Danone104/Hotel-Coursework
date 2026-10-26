using System;
using System.Collections.Generic;
using System.Text;
using Hotel.DataAccess.Configurations;
using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Hotel.DataAccess.DBAdapter
{
    public class HotelDbContext(DbContextOptions<HotelDbContext> options)
        : DbContext(options)
    {   
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RentRoom> Rents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new RentConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }

}

