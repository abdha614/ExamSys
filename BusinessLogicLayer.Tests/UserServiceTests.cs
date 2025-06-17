//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using BusinessLogicLayer.Dtos;
//using BusinessLogicLayer.Interfaces;
//using BusinessLogicLayer.Services;
//using DataAccessLayer.Interfaces;
//using DataAccessLayer.Models;
//using Microsoft.AspNetCore.Identity;
//using Moq;
//using Xunit;

//namespace BusinessLogicLayer.Tests
//{
//    public class UserServiceTests
//    {
//        private readonly Mock<IUserRepository> _userRepositoryMock;
//        private readonly Mock<IHistoryService> _historyServiceMock;
//        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
//        private readonly IMapper _mapper;
//        private readonly UserService _userService;

//        public UserServiceTests()
//        {
//            _userRepositoryMock = new Mock<IUserRepository>();
//            _historyServiceMock = new Mock<IHistoryService>();
//            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<User, UserDto>();
//                cfg.CreateMap<UserDto, User>();
//                cfg.CreateMap<UserRegistrationDto, User>();
//            });

//            _mapper = config.CreateMapper();
//            _userService = new UserService(_userRepositoryMock.Object, _mapper, _historyServiceMock.Object, _passwordHasherMock.Object);
//        }

//        [Fact]
//        public async Task GetUserByIdAsync_ReturnsUser()
//        {
//            // Arrange
//            var user = new User { Id = 1, Email = "test@example.com" };
//            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

//            // Act
//            var result = await _userService.GetUserByIdAsync(1);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(1, result.Id);
//            Assert.Equal("test@example.com", result.Email);
//        }

//        [Fact]
//        public async Task GetUserByEmailAsync_ReturnsUser()
//        {
//            // Arrange
//            var user = new User { Id = 1, Email = "test@example.com" };
//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync("test@example.com")).ReturnsAsync(user);

//            // Act
//            var result = await _userService.GetUserByEmailAsync("test@example.com");

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(1, result.Id);
//            Assert.Equal("test@example.com", result.Email);
//        }

//        [Fact]
//        public async Task GetAllUsersAsync_ReturnsAllUsers()
//        {
//            // Arrange
//            var users = new List<User>
//            {
//                new User { Id = 1, Email = "test1@example.com" },
//                new User { Id = 2, Email = "test2@example.com" }
//            };
//            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

//            // Act
//            var result = await _userService.GetAllUsersAsync();

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(2, result.Count());
//            Assert.Equal("test1@example.com", result.First().Email);
//            Assert.Equal("test2@example.com", result.Last().Email);
//        }

//        [Fact]
//        public async Task DeleteUserAsync_DeletesUser()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

//            // Act
//            await _userService.DeleteUserAsync(1);

//            // Assert
//            _userRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
//        }

//        [Fact]
//        public async Task RegisterUserAsync_AddsNewUser()
//        {
//            // Arrange
//            var registrationDto = new UserRegistrationDto
//            {
//                Email = "newuser@example.com",
//                Password = "password"
//            };

//            var user = new User
//            {
//                Id = 1,
//                Email = "newuser@example.com"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registrationDto.Email))
//                               .ReturnsAsync((User)null); // No existing user with this email
//            _passwordHasherMock.Setup(hasher => hasher.HashPassword(It.IsAny<User>(), registrationDto.Password))
//                               .Returns("hashedPassword");
//            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
//            _historyServiceMock.Setup(service => service.LogActionAsync(It.IsAny<HistoryDto>()))
//                               .Returns(Task.CompletedTask);

//            // Act
//            var result = await _userService.RegisterUserAsync(registrationDto);

//            // Assert
//            _userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(registrationDto.Email), Times.Once);
//            _passwordHasherMock.Verify(hasher => hasher.HashPassword(It.IsAny<User>(), registrationDto.Password), Times.Once);
//            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
//            Assert.NotNull(result);
//            Assert.Equal("newuser@example.com", result.Email);
//        }

//        [Fact]
//        public async Task RegisterUserAsync_UserAlreadyExists_ThrowsInvalidOperationException()
//        {
//            // Arrange
//            var registrationDto = new UserRegistrationDto
//            {
//                Email = "existinguser@example.com",
//                Password = "password"
//            };

//            var existingUser = new User
//            {
//                Id = 1,
//                Email = "existinguser@example.com"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(registrationDto.Email))
//                               .ReturnsAsync(existingUser);

//            // Act & Assert
//            await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUserAsync(registrationDto));

//            _userRepositoryMock.Verify(repo => repo.GetUserByEmailAsync(registrationDto.Email), Times.Once);
//            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
//            _historyServiceMock.Verify(service => service.LogActionAsync(It.IsAny<HistoryDto>()), Times.Never);
//        }

//        [Fact]
//        public async Task GetUsersFilteredAsync_ReturnsFilteredUsers()
//        {
//            // Arrange
//            var users = new List<User>
//            {
//                new User { Id = 1, Email = "test1@example.com", RoleId = 1 },
//                new User { Id = 2, Email = "test2@example.com", RoleId = 2 }
//            };
//            _userRepositoryMock.Setup(repo => repo.GetUsersByRoleAsync(1)).ReturnsAsync(users.Where(u => u.RoleId == 1));

//            // Act
//            var result = await _userService.GetUsersFilteredAsync(1);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Single(result);
//            Assert.Equal("test1@example.com", result.First().Email);
//        }
//    }
//}












