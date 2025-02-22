using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using DataAccessLayer.Models;

namespace DataAccessLayer.Tests
{
    public class GenericRepositoryTests
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
            var sampleEntities = new List<SampleEntity>
            {
                new SampleEntity { Id = 1, Name = "Entity1" },
                new SampleEntity { Id = 2, Name = "Entity2" }
            };

            context.Set<SampleEntity>().AddRange(sampleEntities);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetByIdAsync");
            var genericRepository = new GenericRepository<SampleEntity>(context);
            SeedDatabase(context);

            // Act
            var result = await genericRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Entity1", result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetAllAsync");
            var genericRepository = new GenericRepository<SampleEntity>(context);
            SeedDatabase(context);

            // Act
            var result = await genericRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Name == "Entity1");
            Assert.Contains(result, e => e.Name == "Entity2");
        }

        [Fact]
        public async Task AddAsync_AddsNewEntity()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var genericRepository = new GenericRepository<SampleEntity>(context);
            SeedDatabase(context);
            var newEntity = new SampleEntity { Id = 3, Name = "Entity3" };

            // Act
            await genericRepository.AddAsync(newEntity);
            var result = await genericRepository.GetByIdAsync(3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Entity3", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var genericRepository = new GenericRepository<SampleEntity>(context);
            SeedDatabase(context);
            var existingEntity = await genericRepository.GetByIdAsync(1);
            existingEntity.Name = "Updated Entity";

            // Act
            await genericRepository.UpdateAsync(existingEntity);
            var result = await genericRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Entity", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesEntity()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var genericRepository = new GenericRepository<SampleEntity>(context);
            SeedDatabase(context);

            // Act
            await genericRepository.DeleteAsync(1);
            var result = await genericRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
