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
    public class CategoryRepositoryTests
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
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Math", ProfessorId = 1 },
                new Category { Id = 2, Name = "Geography", ProfessorId = 1 },
                new Category { Id = 3, Name = "Physics", ProfessorId = 2 }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsCategory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetByNameAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);

            // Act
            var result = await categoryRepository.GetByNameAsync("Math");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Math", result.Name);
        }

        [Fact]
        public async Task GetByProfessorIdAsync_ReturnsCategories()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetByProfessorIdAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);

            // Act
            var result = await categoryRepository.GetByProfessorIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Math");
            Assert.Contains(result, c => c.Name == "Geography");
        }

        [Fact]
        public async Task GetCategoryByNameAndProfessorAsync_ReturnsCategory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetCategoryByNameAndProfessorAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);

            // Act
            var result = await categoryRepository.GetCategoryByNameAndProfessorAsync("Math", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Math", result.Name);
            Assert.Equal(1, result.ProfessorId);
        }

        [Fact]
        public async Task AddAsync_AddsNewCategory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);
            var newCategory = new Category { Id = 4, Name = "Chemistry", ProfessorId = 2 };

            // Act
            await categoryRepository.AddAsync(newCategory);
            var result = await categoryRepository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Chemistry", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesCategory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);
            var existingCategory = await categoryRepository.GetByIdAsync(1);
            existingCategory.Name = "Advanced Math";

            // Act
            await categoryRepository.UpdateAsync(existingCategory);
            var result = await categoryRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Advanced Math", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesCategory()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var categoryRepository = new CategoryRepository(context);
            SeedDatabase(context);

            // Act
            await categoryRepository.DeleteAsync(1);
            var result = await categoryRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
