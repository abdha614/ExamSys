using System;
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
    public class HistoryServiceTests
    {
        private readonly Mock<IHistoryRepository> _historyRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HistoryService _historyService;

        public HistoryServiceTests()
        {
            _historyRepositoryMock = new Mock<IHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _historyService = new HistoryService(_historyRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task LogActionAsync_LogsAction()
        {
            // Arrange
            var historyDto = new HistoryDto
            {
                UserId = 1,
                Action = "User Registered",
                IpAddress = "127.0.0.1"
            };

            var history = new History
            {
                UserId = 1,
                Action = "User Registered",
                Timestamp = DateTime.UtcNow, // This will be set in the method
                IpAddress = "127.0.0.1"
            };

            _mapperMock.Setup(mapper => mapper.Map<History>(It.IsAny<HistoryDto>())).Returns(history);
            _historyRepositoryMock.Setup(repo => repo.AddHistoryAsync(It.IsAny<History>())).Returns(Task.CompletedTask);

            // Act
            await _historyService.LogActionAsync(historyDto);

            // Assert
            _historyRepositoryMock.Verify(repo => repo.AddHistoryAsync(It.Is<History>(h =>
                h.UserId == historyDto.UserId &&
                h.Action == historyDto.Action &&
                h.IpAddress == historyDto.IpAddress &&
                h.Timestamp.Date == DateTime.UtcNow.Date)), Times.Once);
        }

        [Fact]
        public async Task GetHistoryAsync_ReturnsHistoryDtos()
        {
            // Arrange
            var historyEntities = new List<History>
        {
            new History { UserId = 1, Action = "Login", Timestamp = DateTime.UtcNow, IpAddress = "127.0.0.1" },
            new History { UserId = 2, Action = "Logout", Timestamp = DateTime.UtcNow, IpAddress = "127.0.0.1" }
        };

            var historyDtos = new List<HistoryDto>
        {
            new HistoryDto { UserId = 1, Action = "Login", Timestamp = DateTime.UtcNow, IpAddress = "127.0.0.1" },
            new HistoryDto { UserId = 2, Action = "Logout", Timestamp = DateTime.UtcNow, IpAddress = "127.0.0.1" }
        };

            _historyRepositoryMock.Setup(repo => repo.GetAllHistoryAsync()).ReturnsAsync(historyEntities);
            _mapperMock.Setup(mapper => mapper.Map<List<HistoryDto>>(historyEntities)).Returns(historyDtos);

            // Act
            var result = await _historyService.GetHistoryAsync();

            // Assert
            _historyRepositoryMock.Verify(repo => repo.GetAllHistoryAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<List<HistoryDto>>(historyEntities), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(historyDtos, result);
        }
    }
}