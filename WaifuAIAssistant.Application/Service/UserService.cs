using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.Enums;
using WaifuAIAssistant.Domain.Services;
using WaifuAIAssistant.Domain.ThirdPartyInterface;
using WaifuAIAssistant.Infrastructure.ThirdParty;
using WaifuAIAssistant.Service.DTOs.Request;
using WaifuAIAssistant.Service.DTOs.Response;

namespace WaifuAIAssistant.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHandlerService _passwordHandlerService;
        private readonly IJwtService _jWTService;
        private readonly GoogleService _googleService;

        public UserService(IUnitOfWork unitOfWork, IPasswordHandlerService passwordHandlerService, IJwtService jWTService, GoogleService googleService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHandlerService = passwordHandlerService ?? throw new ArgumentNullException(nameof(passwordHandlerService));
            _jWTService=jWTService;
            _googleService=googleService;
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
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;
            newUser.PasswordHash = _passwordHandlerService.HashPassword(request.Password);
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "Registration successful",
            };
        }

        public async Task<ApiResponse<string>> VerifyAccount(VerifyAccountRequest request)
        {
            var isOtpValid = await _googleService.VerifyOtp(request.Email, request.Otp);

            if (!isOtpValid)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid OTP",
                    Errors = new List<string> { "Invalid OTP" }
                };
            }

            var user = await findUserByEmail(request.Email);
            if (user == null)
            {
                return new ApiResponse<string>{ Success = false, Message = "Invalid OTP", Errors = new List<string> { "Invalid OTP" } };
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
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Email not found",
                };
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

            // Generate JWT token (optional, if you want to return it)
            var jwtToken = await _jWTService.GenerateJwtToken(user);

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponse
                {
                    Token = jwtToken, 
                    refreshToken = user.RefreshToken
                }
            };
        }

        public async Task<ApiResponse<string>> RefreshToken(RefreshTokenRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync(u => u.RefreshToken == request.Token);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid refresh token",
                    Errors = new List<string> { "Invalid refresh token" }
                };
            }
            // Generate new JWT token
            var jwtToken = await _jWTService.GenerateJwtToken(user);
            // Optionally, you can also generate a new refresh token and update the user's record
            user.RefreshToken = await _jWTService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set new refresh token expiry time
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = jwtToken
            };
        }
    }
}
