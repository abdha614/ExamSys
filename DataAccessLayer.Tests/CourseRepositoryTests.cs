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
    public class CourseRepositoryTests
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

            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Algebra", CategoryId = 1, ProfessorId = 1 },
                new Course { Id = 2, Name = "World Geography", CategoryId = 2, ProfessorId = 1 },
                new Course { Id = 3, Name = "Advanced Physics", CategoryId = 3, ProfessorId = 2 }
            };

            context.Categories.AddRange(categories);
            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetCoursesByCategoryAndProfessorAsync_ReturnsCourses()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetCoursesByCategoryAndProfessorAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            var result = await courseRepository.GetCoursesByCategoryAndProfessorAsync("Math", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Algebra", result.First().Name);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetByNameAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            var result = await courseRepository.GetByNameAsync("Algebra");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Algebra", result.Name);
        }

        [Fact]
        public async Task GetByProfessorIdAsync_ReturnsCourses()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetByProfessorIdAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            var result = await courseRepository.GetByProfessorIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Algebra");
            Assert.Contains(result, c => c.Name == "World Geography");
        }

        [Fact]
        public async Task GetCourseByNameAndProfessorAsync_ReturnsCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetCourseByNameAndProfessorAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            var result = await courseRepository.GetCourseByNameAndProfessorAsync("Algebra", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Algebra", result.Name);
            Assert.Equal(1, result.ProfessorId);
        }

        [Fact]
        public async Task GetCourseByNameCategoryAndProfessorAsync_ReturnsCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_GetCourseByNameCategoryAndProfessorAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            var result = await courseRepository.GetCourseByNameCategoryAndProfessorAsync("Algebra", 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Algebra", result.Name);
            Assert.Equal(1, result.CategoryId);
            Assert.Equal(1, result.ProfessorId);
        }

        [Fact]
        public async Task AddAsync_AddsNewCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_AddAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);
            var newCourse = new Course { Id = 4, Name = "Chemistry", CategoryId = 3, ProfessorId = 2 };

            // Act
            await courseRepository.AddAsync(newCourse);
            var result = await courseRepository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Chemistry", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_UpdateAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);
            var existingCourse = await courseRepository.GetByIdAsync(1);
            existingCourse.Name = "Advanced Algebra";

            // Act
            await courseRepository.UpdateAsync(existingCourse);
            var result = await courseRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Advanced Algebra", result.Name);
        }

        [Fact]
        public async Task DeleteAsync_DeletesCourse()
        {
            // Arrange
            using var context = CreateContext("TestDatabase_DeleteAsync");
            var courseRepository = new CourseRepository(context);
            SeedDatabase(context);

            // Act
            await courseRepository.DeleteAsync(1);
            var result = await courseRepository.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
