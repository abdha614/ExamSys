using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Moq;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class QuestionTypeServiceTests
    {
        private readonly Mock<IQuestionTypeRepository> _questionTypeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly QuestionTypeService _questionTypeService;

        public QuestionTypeServiceTests()
        {
            _questionTypeRepositoryMock = new Mock<IQuestionTypeRepository>();
            _mapperMock = new Mock<IMapper>();
            _questionTypeService = new QuestionTypeService(_questionTypeRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllQuestionTypesAsync_ReturnsQuestionTypeDtos()
        {
            // Arrange
            var questionTypes = new List<QuestionType>
        {
            new QuestionType { Id = 1, Type = "Multiple Choice" },
            new QuestionType { Id = 2, Type = "True/False" }
        };

            var questionTypeDtos = new List<QuestionTypeDto>
        {
            new QuestionTypeDto { Id = 1, Type = "Multiple Choice" },
            new QuestionTypeDto { Id = 2, Type = "True/False" }
        };

            _questionTypeRepositoryMock.Setup(repo => repo.GetAllQuestionTypesAsync())
                                       .ReturnsAsync(questionTypes);

            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<QuestionTypeDto>>(questionTypes))
                       .Returns(questionTypeDtos);

            // Act
            var result = await _questionTypeService.GetAllQuestionTypesAsync();

            // Assert
            _questionTypeRepositoryMock.Verify(repo => repo.GetAllQuestionTypesAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<IEnumerable<QuestionTypeDto>>(questionTypes), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(questionTypeDtos, result);
        }
    }
}