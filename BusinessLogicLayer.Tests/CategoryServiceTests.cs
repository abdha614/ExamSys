using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Dtos.CategoryDto;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Moq;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly IMapper _mapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryGetDto>();
                cfg.CreateMap<CategoryGetDto, Category>();
            });

            _mapper = config.CreateMapper();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1", ProfessorId = 1 },
                new Category { Id = 2, Name = "Category2", ProfessorId = 2 }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Category1", result.First().Name);
            Assert.Equal("Category2", result.Last().Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1", ProfessorId = 1 };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Category1", result.Name);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1", ProfessorId = 1 };

            _categoryRepositoryMock.Setup(repo => repo.GetByNameAsync("Category1")).ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetCategoryByNameAsync("Category1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Category1", result.Name);
        }

        [Fact]
        public async Task AddCategoryAsync_AddsCategory()
        {
            // Arrange
            var categoryDto = new CategoryGetDto { Name = "NewCategory", professorId = 1 };
            var category = _mapper.Map<Category>(categoryDto);

            _categoryRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

            // Act
            var result = await _categoryService.AddCategoryAsync("NewCategory", 1);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("NewCategory", result.Name);
            Assert.Equal(1, result.professorId);
        }

        [Fact]
        public async Task GetCategoriesByProfessorAsync_ReturnsCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1", ProfessorId = 1 },
                new Category { Id = 2, Name = "Category2", ProfessorId = 1 }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetByProfessorIdAsync(1)).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCategoriesByProfessorAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().professorId);
            Assert.Equal(1, result.Last().professorId);
        }

        [Fact]
        public async Task GetCategoryByNameAndProfessorAsync_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category1", ProfessorId = 1 };

            _categoryRepositoryMock.Setup(repo => repo.GetCategoryByNameAndProfessorAsync("Category1", 1)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetCategoryByNameAndProfessorAsync("Category1", 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Category1", result.Name);
            Assert.Equal(1, result.professorId);
        }
    }
}
