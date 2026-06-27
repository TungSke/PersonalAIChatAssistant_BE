using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Application.Interfaces;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Enums;
using PersonalAIAssistant.Domain.Interfaces;
using PersonalAIAssistant.Domain.Services;
using PersonalAIAssistant.Domain.ThirdPartyInterface;
using PersonalAIAssistant.Infrastructure.ThirdParty;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Service.DTOs.Response;

namespace PersonalAIAssistant.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHandlerService _passwordHandlerService;
        private readonly IJwtService _jWTService;
        private readonly GoogleService _googleService;
        private readonly IAuthCookieService _authCookieService;

        public UserService(IUnitOfWork unitOfWork, IPasswordHandlerService passwordHandlerService, IJwtService jWTService, GoogleService googleService, IAuthCookieService authCookieService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHandlerService = passwordHandlerService ?? throw new ArgumentNullException(nameof(passwordHandlerService));
            _jWTService=jWTService;
            _googleService=googleService;
            _authCookieService = authCookieService ?? throw new ArgumentNullException(nameof(authCookieService));
        }

        private static LoginResponse MapUser(User user)
        {
            return new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        private async Task<User> findUserByEmail(string email)
        {
            return await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> findUserById(int id)
        {
            return await _unitOfWork.UserRepository.FindAsync(id);
        }

        public async Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            var userExsisted = await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(u => u.Email == request.Email);
            if (userExsisted != null)
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Email already exists",
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Passwords do not match",
                    Errors = new List<string> { "Passwords do not match" }
                };
            }

            var newUser = request.Adapt<User>();
            newUser.Id = new int();
            newUser.CreatedAt = DateTime.Now;
            newUser.UpdatedAt = DateTime.Now;
            newUser.Status = UserStatus.Inactive;
            newUser.PasswordHash = _passwordHandlerService.HashPassword(request.Password);
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            await _googleService.SendOtpAsync(newUser.Email);

            return new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "Registration successful",
            };
        }

        public async Task<ApiResponse<string>> VerifyAccount(VerifyAccountRequest request)
        {
            var isOtpValid = await _googleService.VerifyOtpAsync(request.Email, request.Otp);

            if (isOtpValid == false)
            {
                throw new UnauthorizedAccessException("Invalid OTP");
            }

            var user = await findUserByEmail(request.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("Email not found");
            }

            user.Status = UserStatus.Active;

            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<string> { Success = true, Message = "verify success"};
        }



        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            var user = await findUserByEmail(request.Email);

            if (user == null)
            {
                throw new KeyNotFoundException("Email not found");
            }

            if(user.Status != UserStatus.Active)
            {
                throw new Exception("Account is not active");
            }

            var passwordVerificationResult =  _passwordHandlerService.VerifyPassword(user.PasswordHash, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            user.RefreshToken = await _jWTService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set refresh token expiry time
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var jwtToken = await _jWTService.GenerateJwtToken(user);
            _authCookieService.SetAuthCookies(jwtToken, user.RefreshToken);

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = MapUser(user)
            };
        }

        public async Task<ApiResponse<LoginResponse>> Me()
        {
            var userId = await _jWTService.GetUserId();
            if (userId <= 0)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Unauthorized"
                };
            }

            var user = await findUserById(userId);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Session restored",
                Data = MapUser(user)
            };
        }

        public async Task<ApiResponse<string>> RefreshToken()
        {
            var refreshToken = _authCookieService.GetRefreshToken();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Missing refresh token"
                };
            }

            var user = await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            var jwtToken = await _jWTService.GenerateJwtToken(user);
            user.RefreshToken = await _jWTService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set new refresh token expiry time
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _authCookieService.SetAuthCookies(jwtToken, user.RefreshToken);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Token refreshed successfully"
            };
        }

        public async Task<ApiResponse<string>> Logout()
        {
            var refreshToken = _authCookieService.GetRefreshToken();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var user = await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            _authCookieService.ClearAuthCookies();

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Logout successful"
            };
        }
    }
}
