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
    public class UserRepositoryTests
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
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Professor" },
                new Role { Id = 3, Name = "Student" }
            };

            var users = new List<User>
            {
                new User { Id = 1, Email = "admin@example.com", PasswordHash = "hash1", RoleId = 1 },
                new User { Id = 2, Email = "professor1@example.com", PasswordHash = "hash2", RoleId = 2 },
                new User { Id = 3, Email = "professor2@example.com", PasswordHash = "hash3", RoleId = 2 },
                new User { Id = 4, Email = "student@example.com", PasswordHash = "hash4", RoleId = 3 }
            };

            context.Roles.AddRange(roles);
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsUser()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetUserByEmailAsync");
            var userRepository = new UserRepository(context);
            SeedDatabase(context);

            // Act
            var result = await userRepository.GetUserByEmailAsync("admin@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("admin@example.com", result.Email);
            Assert.Equal(1, result.RoleId);
        }

        [Fact]
        public async Task GetUsersByRoleAsync_ReturnsUsers()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetUsersByRoleAsync");
            var userRepository = new UserRepository(context);
            SeedDatabase(context);

            // Act
            var result = await userRepository.GetUsersByRoleAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Email == "professor1@example.com");
            Assert.Contains(result, u => u.Email == "professor2@example.com");
        }

        [Fact]
        public async Task AddAsync_AddsNewUser()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var userRepository = new UserRepository(context);
            SeedDatabase(context);
            var newUser = new User { Id = 5, Email = "newuser@example.com", PasswordHash = "hash5", RoleId = 3 };

            // Act
            await userRepository.AddAsync(newUser);
            var result = await userRepository.GetByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.Equal(3, result.RoleId);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUser()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var userRepository = new UserRepository(context);
            SeedDatabase(context);
            var existingUser = await userRepository.GetByIdAsync(1);
            existingUser.Email = "updated@example.com";

            // Act
            await userRepository.UpdateAsync(existingUser);
            var result = await userRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var userRepository = new UserRepository(context);
            SeedDatabase(context);

            // Act
            await userRepository.DeleteAsync(1);
            var result = await userRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
