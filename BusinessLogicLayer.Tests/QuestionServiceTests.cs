//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using BusinessLogicLayer.Dtos.AnswerDto;
//using BusinessLogicLayer.Dtos;
//using BusinessLogicLayer.Dtos.QuestionDto;
//using BusinessLogicLayer.Interfaces;
//using BusinessLogicLayer.Services;
//using DataAccessLayer.Interfaces;
//using DataAccessLayer.Models;
//using Moq;
//using Xunit;

//namespace BusinessLogicLayer.Tests
//{
//    public class QuestionServiceTests
//    {
//        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
//        private readonly Mock<IAnswerRepository> _answerRepositoryMock;
//        private readonly IMapper _mapper;
//        private readonly QuestionService _questionService;

//        public QuestionServiceTests()
//        {
//            _questionRepositoryMock = new Mock<IQuestionRepository>();
//            _answerRepositoryMock = new Mock<IAnswerRepository>();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<Question, QuestionGetDto>();
//                cfg.CreateMap<QuestionAddDto, Question>();
//                cfg.CreateMap<QuestionUpdateDto, Question>();
//                cfg.CreateMap<AnswerAddDto, Answer>();
//                cfg.CreateMap<Answer, AnswerGetDto>();
//            });

//            _mapper = config.CreateMapper();
//            _questionService = new QuestionService(_questionRepositoryMock.Object, _answerRepositoryMock.Object, _mapper);
//        }

//        [Fact]
//        public async Task GetQuestionByIdAsync_ReturnsQuestion()
//        {
//            // Arrange
//            var question = new Question
//            {
//                Id = 1,
//                Text = "Sample Question",
//                CategoryId = 1,
//                CourseId = 1,
//                DifficultyLevelId = 1,
//                Answers = new List<Answer>
//                {
//                    new Answer { Id = 1, Text = "Answer 1", IsCorrect = true },
//                    new Answer { Id = 2, Text = "Answer 2", IsCorrect = false }
//                }
//            };

//            _questionRepositoryMock.Setup(repo => repo.GetQuestionByIdAsync(1)).ReturnsAsync(question);

//            // Act
//            var result = await _questionService.GetQuestionByIdAsync(1);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Sample Question", result.Text);
//            Assert.Equal(2, result.Answers.Count);
//            Assert.Equal("Answer 1", result.Answers.First().Text);
//            Assert.Equal("Answer 2", result.Answers.Last().Text);
//        }

//        [Fact]
//        public async Task AddQuestionAsync_AddsQuestion()
//        {
//            // Arrange
//            var questionDto = new QuestionAddDto
//            {
//                Text = "New Question",
//                CategoryId = 1,
//                CourseId = 1,
//                DifficultyLevelId = 1,
//                Answers = new List<AnswerAddDto>
//                {
//                    new AnswerAddDto { Text = "Answer 1", IsCorrect = true },
//                    new AnswerAddDto { Text = "Answer 2", IsCorrect = false }
//                }
//            };

//            _questionRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

//            // Act
//            await _questionService.AddQuestionAsync(questionDto);

//            // Assert
//            _questionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Question>()), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateQuestionAsync_UpdatesQuestion()
//        {
//            // Arrange
//            var question = new Question
//            {
//                Id = 1,
//                Text = "Existing Question",
//                CategoryId = 1,
//                CourseId = 1,
//                DifficultyLevelId = 1,
//                Answers = new List<Answer>
//                {
//                    new Answer { Id = 1, Text = "Answer 1", IsCorrect = true },
//                    new Answer { Id = 2, Text = "Answer 2", IsCorrect = false }
//                }
//            };

//            var questionUpdateDto = new QuestionUpdateDto
//            {
//                Id = 1,
//                Text = "Updated Question",
//                CategoryId = 2,
//                CourseId = 2,
//                DifficultyLevelId = 2,
//                Answers = new List<AnswerAddDto>
//                {
//                    new AnswerAddDto { Text = "Updated Answer 1", IsCorrect = false },
//                    new AnswerAddDto { Text = "Updated Answer 2", IsCorrect = true }
//                }
//            };

//            _questionRepositoryMock.Setup(repo => repo.GetQuestionByIdAsync(1)).ReturnsAsync(question);
//            _questionRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

//            // Act
//            await _questionService.UpdateQuestionAsync(questionUpdateDto);

//            // Assert
//            _questionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Question>()), Times.Once);
//        }

//        [Fact]
//        public async Task DeleteQuestionAsync_DeletesQuestion()
//        {
//            // Arrange
//            _questionRepositoryMock.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

//            // Act
//            await _questionService.DeleteQuestionAsync(1);

//            // Assert
//            _questionRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
//        }

//        [Fact]
//        public async Task GetQuestionsFilteredAsync_ReturnsQuestions()
//        {
//            // Arrange
//            var questions = new List<Question>
//            {
//                new Question
//                {
//                    Id = 1,
//                    Text = "Sample Question",
//                    CategoryId = 1,
//                    CourseId = 1,
//                    DifficultyLevelId = 1,
//                    Answers = new List<Answer>
//                    {
//                        new Answer { Id = 1, Text = "Answer 1", IsCorrect = true },
//                        new Answer { Id = 2, Text = "Answer 2", IsCorrect = false }
//                    }
//                }
//            };

//            _questionRepositoryMock.Setup(repo => repo.GetQuestionsFilteredAsync(1, null, null, null, null)).ReturnsAsync(questions);

//            // Act
//            var result = await _questionService.GetQuestionsFilteredAsync(1);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Single(result);
//            Assert.Equal("Sample Question", result.First().Text);
//        }
//    }
//}
