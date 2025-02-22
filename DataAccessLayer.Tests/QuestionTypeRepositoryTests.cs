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
    public class QuestionTypeRepositoryTests
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
            var questionTypes = new List<QuestionType>
            {
                new QuestionType { Id = 1, Type = "Multiple Choice" },
                new QuestionType { Id = 2, Type = "True/False" },
                new QuestionType { Id = 3, Type = "Essay" }
            };

            context.QuestionTypes.AddRange(questionTypes);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllQuestionTypesAsync_ReturnsAllQuestionTypes()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetAllQuestionTypesAsync");
            var questionTypeRepository = new QuestionTypeRepository(context);
            SeedDatabase(context);

            // Act
            var result = await questionTypeRepository.GetAllQuestionTypesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, qt => qt.Type == "Multiple Choice");
            Assert.Contains(result, qt => qt.Type == "True/False");
            Assert.Contains(result, qt => qt.Type == "Essay");
        }

        [Fact]
        public async Task AddAsync_AddsNewQuestionType()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var questionTypeRepository = new QuestionTypeRepository(context);
            SeedDatabase(context);
            var newQuestionType = new QuestionType { Id = 4, Type = "Short Answer" };

            // Act
            await questionTypeRepository.AddAsync(newQuestionType);
            var result = await questionTypeRepository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Short Answer", result.Type);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesQuestionType()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var questionTypeRepository = new QuestionTypeRepository(context);
            SeedDatabase(context);
            var existingQuestionType = await questionTypeRepository.GetByIdAsync(1);
            existingQuestionType.Type = "Updated Multiple Choice";

            // Act
            await questionTypeRepository.UpdateAsync(existingQuestionType);
            var result = await questionTypeRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Multiple Choice", result.Type);
        }

        [Fact]
        public async Task DeleteAsync_DeletesQuestionType()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var questionTypeRepository = new QuestionTypeRepository(context);
            SeedDatabase(context);

            // Act
            await questionTypeRepository.DeleteAsync(1);
            var result = await questionTypeRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
