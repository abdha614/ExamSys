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
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _roleService = new RoleService(_roleRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ReturnsRoleDto()
        {
            // Arrange
            var roleId = 1;
            var role = new Role { Id = roleId, Name = "Admin" };
            var roleDto = new RoleDto { Id = roleId, Name = "Admin" };

            _roleRepositoryMock.Setup(repo => repo.GetByIdAsync(roleId)).ReturnsAsync(role);
            _mapperMock.Setup(mapper => mapper.Map<RoleDto>(role)).Returns(roleDto);

            // Act
            var result = await _roleService.GetRoleByIdAsync(roleId);

            // Assert
            _roleRepositoryMock.Verify(repo => repo.GetByIdAsync(roleId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<RoleDto>(role), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(roleDto, result);
        }

        [Fact]
        public async Task GetAllRolesAsync_ReturnsRoleDtos()
        {
            // Arrange
            var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        };

            var roleDtos = new List<RoleDto>
        {
            new RoleDto { Id = 1, Name = "Admin" },
            new RoleDto { Id = 2, Name = "User" }
        };

            _roleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(roles);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<RoleDto>>(roles)).Returns(roleDtos);

            // Act
            var result = await _roleService.GetAllRolesAsync();

            // Assert
            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<IEnumerable<RoleDto>>(roles), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(roleDtos, result);
        }
    }
}