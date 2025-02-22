using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CourseDto;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Moq;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly IMapper _mapper;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _categoryServiceMock = new Mock<ICategoryService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Course, CourseGetDto>();
                cfg.CreateMap<CourseGetDto, Course>();
            });

            _mapper = config.CreateMapper();
            _courseService = new CourseService(_courseRepositoryMock.Object, _categoryServiceMock.Object, _mapper);
        }

        [Fact]
        public async Task GetCoursesByProfessorAsync_ReturnsCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Course1", CategoryId = 1, ProfessorId = 1 },
                new Course { Id = 2, Name = "Course2", CategoryId = 2, ProfessorId = 1 }
            };

            _courseRepositoryMock.Setup(repo => repo.GetByProfessorIdAsync(1)).ReturnsAsync(courses);

            // Act
            var result = await _courseService.GetCoursesByProfessorAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Course1", result.First().Name);
            Assert.Equal("Course2", result.Last().Name);
        }

        [Fact]
        public async Task GetAllCoursesAsync_ReturnsAllCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Course1", CategoryId = 1, ProfessorId = 1 },
                new Course { Id = 2, Name = "Course2", CategoryId = 2, ProfessorId = 2 }
            };

            _courseRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(courses);

            // Act
            var result = await _courseService.GetAllCoursesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Course1", result.First().Name);
            Assert.Equal("Course2", result.Last().Name);
        }

        [Fact]
        public async Task GetAllCoursesByCategoryAndProfessorAsync_ReturnsCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { Id = 1, Name = "Course1", CategoryId = 1, ProfessorId = 1 },
                new Course { Id = 2, Name = "Course2", CategoryId = 1, ProfessorId = 1 }
            };

            _courseRepositoryMock.Setup(repo => repo.GetCoursesByCategoryAndProfessorAsync("Category1", 1)).ReturnsAsync(courses);

            // Act
            var result = await _courseService.GetAllCoursesByCategoryAndProfessorAsync("Category1", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().CategoryId);
            Assert.Equal(1, result.Last().CategoryId);
        }

        [Fact]
        public async Task GetCourseByNameAsync_ReturnsCourse()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Course1", CategoryId = 1 };

            _courseRepositoryMock.Setup(repo => repo.GetByNameAsync("Course1")).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByNameAsync("Course1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Course1", result.Name);
        }

        [Fact]
        public async Task AddCourseAsync_AddsCourse()
        {
            // Arrange
            var courseDto = new CourseGetDto { Name = "NewCourse", CategoryId = 1 };
            var course = _mapper.Map<Course>(courseDto);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

            // Act
            var result = await _courseService.AddCourseAsync("NewCourse", 1, 1);

            // Assert
            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("NewCourse", result.Name);
            Assert.Equal(1, result.CategoryId);
        }

        [Fact]
        public async Task GetCourseByNameAndProfessorAsync_ReturnsCourse()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Course1", CategoryId = 1 };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAndProfessorAsync("Course1", 1)).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByNameAndProfessorAsync("Course1", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Course1", result.Name);
        }

        [Fact]
        public async Task GetCourseByNameCategoryAndProfessorAsync_ReturnsCourse()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Course1", CategoryId = 1 };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameCategoryAndProfessorAsync("Course1", 1, 1)).ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByNameCategoryAndProfessorAsync("Course1", 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Course1", result.Name);
            Assert.Equal(1, result.CategoryId);
        }
    }
}
