using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalAIAssistant.API.Controllers;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Service.DTOs.Response;
using PersonalAIAssistant.Application.DTOs.Response;
using Xunit;
using PersonalAIAssistant.Application.Interfaces.Services;

namespace PersonalAIAssistant.Test.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        // Test cases for Register and Login methods
        [Fact]
        public async Task Register_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var request = new RegisterRequest { Username = "testuser", Email = "test@example.com", Password = "Password123!", ConfirmPassword = "Password123!" };
            var apiResponse = new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "Registration successful",
                Data = new RegisterResponse() // Giả sử DTO này rỗng hoặc có data
            };

            _userServiceMock.Setup(s => s.Register(It.IsAny<RegisterRequest>()))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeAssignableTo<ApiResponse<RegisterResponse>>().Subject;

            response.Success.Should().BeTrue();
            response.Message.Should().Be("Registration successful");
            _userServiceMock.Verify(s => s.Register(It.IsAny<RegisterRequest>()), Times.Once);
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest { Username = "existinguser", Email = "existing@example.com", Password = "Password123!", ConfirmPassword = "Password123!" };
            var apiResponse = new ApiResponse<RegisterResponse>
            {
                Success = false,
                Message = "Email already exists"
            };

            _userServiceMock.Setup(s => s.Register(It.IsAny<RegisterRequest>()))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeAssignableTo<ApiResponse<RegisterResponse>>().Subject;

            response.Success.Should().BeFalse();
            response.Message.Should().Be("Email already exists");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
            var apiResponse = new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponse { UserId = 1, Username = "testuser", Email = "test@example.com" }
            };

            _userServiceMock.Setup(s => s.Login(It.IsAny<LoginRequest>()))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeAssignableTo<ApiResponse<LoginResponse>>().Subject;

            response.Success.Should().BeTrue();
            var data = response.Data;
            data.Should().NotBeNull();
            data!.UserId.Should().Be(1);
            data.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword!123" };
            var apiResponse = new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Invalid credentials"
            };

            _userServiceMock.Setup(s => s.Login(It.Is<LoginRequest>(x =>
                                                x.Email == request.Email &&
                                                x.Password == request.Password)))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeAssignableTo<ApiResponse<LoginResponse>>().Subject;

            response.Success.Should().BeFalse();
            response.Message.Should().Be("Invalid credentials");
        }
    }
}
