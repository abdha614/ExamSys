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
    public class RoleRepositoryTests
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

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetRoleByNameAsync_ReturnsRole()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetRoleByNameAsync");
            var roleRepository = new RoleRepository(context);
            SeedDatabase(context);

            // Act
            var result = await roleRepository.GetRoleByNameAsync("Admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Admin", result.Name);
        }

        [Fact]
        public async Task AddAsync_AddsNewRole()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var roleRepository = new RoleRepository(context);
            SeedDatabase(context);
            var newRole = new Role { Id = 4, Name = "Researcher" };

            // Act
            await roleRepository.AddAsync(newRole);
            var result = await roleRepository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Researcher", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesRole()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var roleRepository = new RoleRepository(context);
            SeedDatabase(context);
            var existingRole = await roleRepository.GetByIdAsync(1);
            existingRole.Name = "Super Admin";

            // Act
            await roleRepository.UpdateAsync(existingRole);
            var result = await roleRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Super Admin", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesRole()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var roleRepository = new RoleRepository(context);
            SeedDatabase(context);

            // Act
            await roleRepository.DeleteAsync(1);
            var result = await roleRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
