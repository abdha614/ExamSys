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
    public class AnswerRepositoryTests
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
            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    ProfessorId = 1,
                    Text = "What is 2+2?",
                    QuestionTypeId = 1,
                    DifficultyLevelId = 1,
                    CategoryId = 1,
                    CourseId = 1
                },
                new Question
                {
                    Id = 2,
                    ProfessorId = 1,
                    Text = "What is the capital of France?",
                    QuestionTypeId = 2,
                    DifficultyLevelId = 2,
                    CategoryId = 2,
                    CourseId = 2
                }
            };

            var answers = new List<Answer>
            {
                new Answer { Id = 1, Text = "4", IsCorrect = true, QuestionId = 1 },
                new Answer { Id = 2, Text = "3", IsCorrect = false, QuestionId = 1 },
                new Answer { Id = 3, Text = "Paris", IsCorrect = true, QuestionId = 2 },
                new Answer { Id = 4, Text = "London", IsCorrect = false, QuestionId = 2 }
            };

            context.Questions.AddRange(questions);
            context.Answers.AddRange(answers);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAnswersByQuestionIdAsync_ReturnsAnswers()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetAnswersByQuestionIdAsync");
            var answerRepository = new AnswerRepository(context);
            SeedDatabase(context);

            // Act
            var result = await answerRepository.GetAnswersByQuestionIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Text == "4");
            Assert.Contains(result, a => a.Text == "3");
        }

        [Fact]
        public async Task AddAsync_AddsNewAnswer()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var answerRepository = new AnswerRepository(context);
            SeedDatabase(context);
            var newAnswer = new Answer { Id = 5, Text = "Correct Answer", IsCorrect = true, QuestionId = 1 };

            // Act
            await answerRepository.AddAsync(newAnswer);
            var result = await answerRepository.GetByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Correct Answer", result.Text);
            Assert.True(result.IsCorrect);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAnswer()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var answerRepository = new AnswerRepository(context);
            SeedDatabase(context);
            var existingAnswer = await answerRepository.GetByIdAsync(1);
            existingAnswer.Text = "Updated Answer";

            // Act
            await answerRepository.UpdateAsync(existingAnswer);
            var result = await answerRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Answer", result.Text);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAnswer()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var answerRepository = new AnswerRepository(context);
            SeedDatabase(context);

            // Act
            await answerRepository.DeleteAsync(1);
            var result = await answerRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
