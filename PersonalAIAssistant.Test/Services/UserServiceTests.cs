using Moq;
using MockQueryable.Moq;
using Microsoft.AspNetCore.Identity;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;
using PersonalAIAssistant.Application.Services;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Enums;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Request;
using Xunit;
using PersonalAIAssistant.Domain.Repositories;
using MockQueryable;
using Microsoft.Extensions.Configuration;

namespace PersonalAIAssistant.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPasswordHandlerService> _passwordServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IGoogleService> _googleServiceMock;
        private readonly Mock<IAuthCookieService> _cookieServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordServiceMock = new Mock<IPasswordHandlerService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _googleServiceMock = new Mock<IGoogleService>();
            _cookieServiceMock = new Mock<IAuthCookieService>();
            _configurationMock = new Mock<IConfiguration>();

            _userService = new UserService(
                _unitOfWorkMock.Object,
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _googleServiceMock.Object,
                _cookieServiceMock.Object,
                _configurationMock.Object
                );
        }

        #region Register

        [Fact]
        public async Task Register_EmailExists_ReturnsFailure()
        {
            // Arrange
            var existingUser = new User { Id = 1, Email = "test@example.com", Username = "u1" };
            var users = new List<User> { existingUser }.BuildMock();

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            var request = new RegisterRequest { Email = "test@example.com", Username = "u1", Password = "password", ConfirmPassword = "password" };

            // Act
            var result = await _userService.Register(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email already exists", result.Message);
        }

        [Fact]
        public async Task Register_PasswordMismatch_ReturnsFailure()
        {
            // Arrange
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            var request = new RegisterRequest { Email = "new@example.com", Username = "new", Password = "pw1", ConfirmPassword = "pw2" };

            // Act
            var result = await _userService.Register(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Passwords do not match", result.Message);
            Assert.Contains("Passwords do not match", result.Errors);
        }

        [Fact]
        public async Task Register_Success_CallsSendOtp()
        {
            // Arrange
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashed");
            _googleServiceMock.Setup(g => g.SendOtpAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var request = new RegisterRequest { Email = "new@example.com", Username = "new", Password = "password", ConfirmPassword = "password" };

            // Act
            var result = await _userService.Register(request);

            // Assert
            Assert.True(result.Success);
            _googleServiceMock.Verify(g => g.SendOtpAsync("new@example.com"), Times.Once);
            userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region VerifyAccount

        [Fact]
        public async Task VerifyAccount_InvalidOtp_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _googleServiceMock.Setup(g => g.VerifyOtpAsync("a@b.com", "1234")).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.VerifyAccount(new VerifyAccountRequest { Email = "a@b.com", Otp = "1234" }));
        }

        [Fact]
        public async Task VerifyAccount_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            _googleServiceMock.Setup(g => g.VerifyOtpAsync("a@b.com", "1234")).ReturnsAsync(true);
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.VerifyAccount(new VerifyAccountRequest { Email = "a@b.com", Otp = "1234" }));
        }

        [Fact]
        public async Task VerifyAccount_Success_ActivatesUser()
        {
            // Arrange
            var user = new User { Id = 5, Email = "a@b.com", Status = UserStatus.Inactive };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            userRepoMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
            _googleServiceMock.Setup(g => g.VerifyOtpAsync("a@b.com", "1234")).ReturnsAsync(true);

            // Act
            var result = await _userService.VerifyAccount(new VerifyAccountRequest { Email = "a@b.com", Otp = "1234" });

            // Assert
            Assert.True(result.Success);
            Assert.Equal(UserStatus.Active, user.Status);
            userRepoMock.Verify(r => r.Update(It.Is<User>(u => u.Status == UserStatus.Active)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region Login

        [Fact]
        public async Task Login_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.Login(new LoginRequest { Email = "noone@x.com", Password = "p" }));
        }

        [Fact]
        public async Task Login_AccountNotActive_ThrowsException()
        {
            // Arrange
            var user = new User { Id = 2, Email = "x@y.com", PasswordHash = "h", Status = UserStatus.Inactive };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.Login(new LoginRequest { Email = "x@y.com", Password = "p" }));
        }

        [Fact]
        public async Task Login_InvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User { Id = 3, Email = "a@b.com", PasswordHash = "h", Status = UserStatus.Active };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            _passwordServiceMock.Setup(p => p.VerifyPassword(user.PasswordHash, "wrong")).Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.Login(new LoginRequest { Email = "a@b.com", Password = "wrong" }));
        }

        [Fact]
        public async Task Login_Success_ReturnsLoginResponse_And_CallsTokenAndCookie()
        {
            // Arrange
            var user = new User { Id = 4, Email = "ok@ok.com", PasswordHash = "h", Status = UserStatus.Active };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            userRepoMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

            _passwordServiceMock.Setup(p => p.VerifyPassword(user.PasswordHash, "correct")).Returns(PasswordVerificationResult.Success);
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("refresh");
            _tokenServiceMock.Setup(t => t.GenerateJwtToken(user)).ReturnsAsync("jwt");

            // Act
            var result = await _userService.Login(new LoginRequest { Email = "ok@ok.com", Password = "correct" });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Login successful", result.Message);
            Assert.NotNull(result.Data);
            _cookieServiceMock.Verify(c => c.SetAuthCookies("jwt", "refresh"), Times.Once);
            userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region Me

        [Fact]
        public async Task Me_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _tokenServiceMock.Setup(t => t.GetUserId()).Returns(0);

            // Act
            var result = await _userService.Me();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unauthorized", result.Message);
        }

        [Fact]
        public async Task Me_UserNotFound_ReturnsUserNotFound()
        {
            // Arrange
            _tokenServiceMock.Setup(t => t.GetUserId()).Returns(10);
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User?)null);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act
            var result = await _userService.Me();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task Me_Success_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 7, Email = "me@me.com", Username = "me" };
            _tokenServiceMock.Setup(t => t.GetUserId()).Returns(7);
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.FindAsync(It.Is<object[]>(ids => (int)ids[0] == 7))).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act
            var result = await _userService.Me();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("me@me.com", result.Data.Email);
        }

        #endregion

        #region RefreshToken

        [Fact]
        public async Task RefreshToken_MissingCookie_ReturnsFailure()
        {
            // Arrange
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns((string?)null);

            // Act
            var result = await _userService.RefreshToken();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Missing refresh token", result.Message);
        }

        [Fact]
        public async Task RefreshToken_UserNotFound_ReturnsFailure()
        {
            // Arrange
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns("some-token");
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act
            var result = await _userService.RefreshToken();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid or expired refresh token", result.Message);
        }

        [Fact]
        public async Task RefreshToken_ExpiredToken_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                Id = 8,
                Email = "expired@x.com",
                RefreshToken = "expired-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
            };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns("expired-token");

            // Act
            var result = await _userService.RefreshToken();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid or expired refresh token", result.Message);
        }

        [Fact]
        public async Task RefreshToken_Success_RotatesTokenAndSetsCookies()
        {
            // Arrange
            var user = new User
            {
                Id = 9,
                Email = "valid@x.com",
                RefreshToken = "old-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            userRepoMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns("old-token");
            _tokenServiceMock.Setup(t => t.GenerateJwtToken(user)).ReturnsAsync("new-jwt");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("new-refresh");

            // Act
            var result = await _userService.RefreshToken();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Token refreshed successfully", result.Message);
            Assert.Equal("new-refresh", user.RefreshToken);
            _cookieServiceMock.Verify(c => c.SetAuthCookies("new-jwt", "new-refresh"), Times.Once);
            userRepoMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        #endregion

        #region Logout

        [Fact]
        public async Task Logout_NoCookie_StillClearsCookiesAndReturnsSuccess()
        {
            // Arrange
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns((string?)null);

            // Act
            var result = await _userService.Logout();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Logout successful", result.Message);
            _cookieServiceMock.Verify(c => c.ClearAuthCookies(), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task Logout_CookiePresentButUserNotFound_StillClearsCookies()
        {
            // Arrange
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns("orphan-token");
            var users = new List<User>().BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);

            // Act
            var result = await _userService.Logout();

            // Assert
            Assert.True(result.Success);
            _cookieServiceMock.Verify(c => c.ClearAuthCookies(), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task Logout_Success_ClearsRefreshTokenAndCookies()
        {
            // Arrange
            var user = new User
            {
                Id = 11,
                Email = "logout@x.com",
                RefreshToken = "active-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            var users = new List<User> { user }.BuildMock();
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetAll()).Returns(users);
            userRepoMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
            _cookieServiceMock.Setup(c => c.GetRefreshToken()).Returns("active-token");

            // Act
            var result = await _userService.Logout();

            // Assert
            Assert.True(result.Success);
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiryTime);
            userRepoMock.Verify(r => r.Update(It.Is<User>(u => u.RefreshToken == null)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
            _cookieServiceMock.Verify(c => c.ClearAuthCookies(), Times.Once);
        }

        #endregion

        #region GoogleLogin

        //[Fact]
        //public async Task GoogleLogin_InvalidIdToken_ReturnsFailure()
        //{
        //    // Arrange
        //    // GoogleJsonWebSignature.ValidateAsync is a static call from Google.Apis.Auth
        //    // and is not mockable through DI here. A garbage/malformed token reliably
        //    // fails validation and hits the catch branch, so this covers that branch only.
        //    var request = new GoogleLoginRequest { IdToken = "not-a-real-google-token" };

        //    // Act
        //    var result = await _userService.GoogleLogin(request);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.Equal("Invalid Google token", result.Message);
        //}

        // NOTE: the "new user created", "existing user inactive", and "success" branches
        // of GoogleLogin cannot be unit tested as the method is currently written, because
        // GoogleJsonWebSignature.ValidateAsync() is called statically instead of through an
        // injected abstraction. To cover those branches, extract token validation behind an
        // interface (e.g. IGoogleTokenValidator) and inject it into UserService, then mock it.

        #endregion
    }
}