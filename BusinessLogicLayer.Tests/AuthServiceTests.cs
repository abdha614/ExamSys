//using System;
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
//    public class AuthServiceTests
//    {
//        private readonly Mock<IUserRepository> _userRepositoryMock;
//        private readonly Mock<IHistoryService> _historyServiceMock;
//        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
//        private readonly IMapper _mapper;
//        private readonly AuthService _authService;

//        public AuthServiceTests()
//        {
//            _userRepositoryMock = new Mock<IUserRepository>();
//            _historyServiceMock = new Mock<IHistoryService>();
//            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<User, UserDto>();
//            });

//            _mapper = config.CreateMapper();
//            _authService = new AuthService(_userRepositoryMock.Object, _mapper, _passwordHasherMock.Object, _historyServiceMock.Object);
//        }

//        [Fact]
//        public async Task LoginAsync_ValidCredentials_ReturnsUserDto()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = 1,
//                Email = "test@example.com",
//                PasswordHash = "hashedPassword"
//            };

//            var loginDto = new LoginDto
//            {
//                Email = "test@example.com",
//                Password = "password"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);
//            _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
//                .Returns(PasswordVerificationResult.Success);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(user.Id, result.Id);
//            Assert.Equal(user.Email, result.Email);
//        }

//        [Fact]
//        public async Task LoginAsync_InvalidEmail_ThrowsUnauthorizedAccessException()
//        {
//            // Arrange
//            var loginDto = new LoginDto
//            {
//                Email = "invalid@example.com",
//                Password = "password"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
//        }

//        [Fact]
//        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = 1,
//                Email = "test@example.com",
//                PasswordHash = "hashedPassword"
//            };

//            var loginDto = new LoginDto
//            {
//                Email = "test@example.com",
//                Password = "invalidPassword"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);
//            _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
//                .Returns(PasswordVerificationResult.Failed);

//            // Act & Assert
//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginDto));
//        }

//        [Fact]
//        public async Task LoginAsync_ValidCredentials_LogsAction()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = 1,
//                Email = "test@example.com",
//                PasswordHash = "hashedPassword"
//            };

//            var loginDto = new LoginDto
//            {
//                Email = "test@example.com",
//                Password = "password"
//            };

//            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);
//            _passwordHasherMock.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
//                .Returns(PasswordVerificationResult.Success);
//            _historyServiceMock.Setup(service => service.LogActionAsync(It.IsAny<HistoryDto>())).Returns(Task.CompletedTask);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            _historyServiceMock.Verify(service => service.LogActionAsync(It.Is<HistoryDto>(h =>
//                h.UserId == user.Id &&
//                h.Action == "User Logged In" &&
//                h.IpAddress == "172.1.1.1" &&
//                h.Timestamp.Date == DateTime.UtcNow.Date
//            )), Times.Once);
//        }
//    }
//}
