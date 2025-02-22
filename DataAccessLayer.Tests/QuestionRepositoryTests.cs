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
    public class QuestionRepositoryTests
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
        new QuestionType { Id = 2, Type = "True/False" }
    };

            var difficultyLevels = new List<DifficultyLevel>
    {
        new DifficultyLevel { Id = 1, Level = "Easy" },
        new DifficultyLevel { Id = 2, Level = "Medium" },
        new DifficultyLevel { Id = 3, Level = "Hard" }
    };

            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "Math" },
        new Category { Id = 2, Name = "Geography" },
        new Category { Id = 3, Name = "Physics" }
    };

            var courses = new List<Course>
    {
        new Course { Id = 1, Name = "Algebra", CategoryId = 1 },
        new Course { Id = 2, Name = "World Geography", CategoryId = 2 },
        new Course { Id = 3, Name = "Advanced Physics", CategoryId = 3 }
    };

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
            CourseId = 1,
            Answers = new List<Answer> { new Answer { Text = "4", IsCorrect = true } }
        },
        new Question
        {
            Id = 2,
            ProfessorId = 1,
            Text = "What is the capital of France?",
            QuestionTypeId = 2,
            DifficultyLevelId = 2,
            CategoryId = 2,
            CourseId = 2,
            Answers = new List<Answer> { new Answer { Text = "Paris", IsCorrect = true } }
        },
        new Question
        {
            Id = 3,
            ProfessorId = 2,
            Text = "What is the speed of light?",
            QuestionTypeId = 2,
            DifficultyLevelId = 3,
            CategoryId = 3,
            CourseId = 3,
            Answers = new List<Answer> { new Answer { Text = "299,792,458 m/s", IsCorrect = true } }
        }
    };

            context.QuestionTypes.AddRange(questionTypes);
            context.DifficultyLevels.AddRange(difficultyLevels);
            context.Categories.AddRange(categories);
            context.Courses.AddRange(courses);
            context.Questions.AddRange(questions);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetQuestionsByProfessorAsync_ReturnsQuestions()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetQuestionsByProfessorAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);

            // Act
            var result = await questionRepository.GetQuestionsByProfessorAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, q => q.Text == "What is 2+2?");
            Assert.Contains(result, q => q.Text == "What is the capital of France?");
        }



        [Fact]
        public async Task GetQuestionByIdAsync_ReturnsQuestion()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetQuestionByIdAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);

            // Verify data is seeded correctly
            var allQuestions = await context.Questions.ToListAsync();
            foreach (var question in allQuestions)
            {
                System.Diagnostics.Debug.WriteLine($"Seeded Question: ID={question.Id}, Text={question.Text}, ProfessorId={question.ProfessorId}");
            }

            // Act
            var result = await questionRepository.GetQuestionByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("What is 2+2?", result.Text);
            Assert.NotNull(result.QuestionType); // Ensure related entities are loaded
            Assert.NotNull(result.DifficultyLevel);
            Assert.NotNull(result.Category);
            Assert.NotNull(result.Course);
            Assert.NotNull(result.Answers);
        }

        [Fact]
        public async Task GetQuestionsFilteredAsync_ReturnsFilteredQuestions()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetQuestionsFilteredAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);

            // Verify data is seeded correctly
            Assert.Equal(3, await context.Questions.CountAsync());

            // Act
            var result = await questionRepository.GetQuestionsFilteredAsync(1, questionTypeId: 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("What is 2+2?", result.First().Text);
        }

        [Fact]
        public async Task AddAsync_AddsNewQuestion()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);
            var newQuestion = new Question { Id = 4, ProfessorId = 1, Text = "What is the boiling point of water?", QuestionTypeId = 1, DifficultyLevelId = 1, CategoryId = 1, CourseId = 1 };

            // Act
            await questionRepository.AddAsync(newQuestion);
            var result = await questionRepository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("What is the boiling point of water?", result.Text);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesQuestion()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);
            var existingQuestion = await questionRepository.GetByIdAsync(1);
            existingQuestion.Text = "Updated Question";

            // Act
            await questionRepository.UpdateAsync(existingQuestion);
            var result = await questionRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Question", result.Text);
        }

        [Fact]
        public async Task DeleteAsync_DeletesQuestion()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var questionRepository = new QuestionRepository(context);
            SeedDatabase(context);

            // Act
            await questionRepository.DeleteAsync(1);
            var result = await questionRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}