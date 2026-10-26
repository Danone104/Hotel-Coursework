using Hotel.DataAccess.DBAdapter;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.IntegrationTests
{
    public class BaseTest:IDisposable
    {
        protected readonly HotelDbContext _context;
        private readonly SqliteConnection _connection;

        protected BaseTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<HotelDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new HotelDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
