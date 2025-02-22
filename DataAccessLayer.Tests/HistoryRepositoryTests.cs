using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Tests
{
    public class HistoryRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        private ApplicationDbContext CreateContext(string databaseName)
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            return new ApplicationDbContext(_dbContextOptions);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            var users = new List<User>
            {
                new User { Id = 1, Email = "user1@example.com", PasswordHash = "hash1", RoleId = 1 },
                new User { Id = 2, Email = "user2@example.com", PasswordHash = "hash2", RoleId = 2 }
            };

            var histories = new List<History>
            {
                new History { Id = 1, UserId = 1, Action = "Login", Timestamp = DateTime.UtcNow.AddHours(-1), IpAddress = "192.168.1.1" },
                new History { Id = 2, UserId = 1, Action = "Logout", Timestamp = DateTime.UtcNow.AddMinutes(-30), IpAddress = "192.168.1.1" },
                new History { Id = 3, UserId = 2, Action = "Login", Timestamp = DateTime.UtcNow.AddHours(-2), IpAddress = "192.168.1.2" }
            };

            context.Users.AddRange(users);
            context.Histories.AddRange(histories);
            context.SaveChanges();
        }

        [Fact]
        public async Task AddHistoryAsync_AddsNewHistory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddHistoryAsync");
            var historyRepository = new HistoryRepository(context);
            SeedDatabase(context);
            var newHistory = new History { Id = 4, UserId = 1, Action = "Purchase", Timestamp = DateTime.UtcNow, IpAddress = "192.168.1.1" };

            // Act
            await historyRepository.AddHistoryAsync(newHistory);
            var result = await context.Histories.FindAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Purchase", result.Action);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetAllHistoryAsync_ReturnsAllHistories()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetAllHistoryAsync");
            var historyRepository = new HistoryRepository(context);
            SeedDatabase(context);

            // Act
            var result = await historyRepository.GetAllHistoryAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, h => h.Action == "Login");
            Assert.Contains(result, h => h.Action == "Logout");
            Assert.Contains(result, h => h.Action == "Login");
        }
    }
}
