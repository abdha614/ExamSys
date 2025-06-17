//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using BusinessLogicLayer.Dtos;
//using BusinessLogicLayer.Dtos.CategoryDto;
//using BusinessLogicLayer.Dtos.CourseDto;
//using BusinessLogicLayer.Dtos.QuestionDto;
//using BusinessLogicLayer.Interfaces;
//using Moq;
//using Xunit;

//namespace BusinessLogicLayer.Tests
//{
//    public class CsvImportServiceTests
//    {
//        private readonly Mock<ICategoryService> _categoryServiceMock;
//        private readonly Mock<ICourseService> _courseServiceMock;
//        private readonly Mock<IQuestionService> _questionServiceMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly CsvImportService _csvImportService;

//        public CsvImportServiceTests()
//        {
//            _categoryServiceMock = new Mock<ICategoryService>();
//            _courseServiceMock = new Mock<ICourseService>();
//            _questionServiceMock = new Mock<IQuestionService>();
//            _mapperMock = new Mock<IMapper>();
//            _csvImportService = new CsvImportService(_categoryServiceMock.Object, _courseServiceMock.Object, _questionServiceMock.Object, _mapperMock.Object);
//        }

//        [Fact]
//        public async Task ImportQuestionsAsync_ValidCsv_ImportsQuestions()
//        {
//            // Arrange
//            var csvContent = new StringBuilder();
//            csvContent.AppendLine("Category,Course,QuestionText,DifficultyLevel,QuestionType,Answer1,Answer2,Answer3,Answer4,CorrectAnswerIndex");
//            csvContent.AppendLine("Math,Algebra,What is 2+2?,Easy,MultipleChoice,2,3,4,5,2");

//            var importStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent.ToString()));

//            var category = new CategoryGetDto { Id = 1, Name = "Math" };
//            var course = new CourseGetDto { Id = 1, Name = "Algebra", CategoryId = 1 };

//            _categoryServiceMock.Setup(service => service.GetCategoryByNameAndProfessorAsync(It.IsAny<string>(), It.IsAny<int>()))
//                                .ReturnsAsync(category);

//            _courseServiceMock.Setup(service => service.GetCourseByNameAndProfessorAsync(It.IsAny<string>(), It.IsAny<int>()))
//                              .ReturnsAsync(course);

//            _questionServiceMock.Setup(service => service.AddQuestionAsync(It.IsAny<QuestionAddDto>()))
//                                .Returns(Task.CompletedTask);

//            // Act
//            await _csvImportService.ImportQuestionsAsync(importStream, "questions.csv", 1);

//            // Assert
//            _categoryServiceMock.Verify(service => service.GetCategoryByNameAndProfessorAsync("Math", 1), Times.Once);
//            _courseServiceMock.Verify(service => service.GetCourseByNameAndProfessorAsync("Algebra", 1), Times.Once);
//            _questionServiceMock.Verify(service => service.AddQuestionAsync(It.IsAny<QuestionAddDto>()), Times.Once);
//        }
//    }
//}