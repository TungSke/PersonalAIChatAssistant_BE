using Google.Apis.Auth;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;
using PersonalAIAssistant.Application.Interfaces.Services;
using PersonalAIAssistant.Domain;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Enums;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Service.DTOs.Response;

namespace PersonalAIAssistant.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHandlerService _passwordHandlerService;
        private readonly ITokenService _tokenService;
        private readonly IGoogleService _googleService;
        private readonly IAuthCookieService _authCookieService;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IPasswordHandlerService passwordHandlerService, ITokenService tokenService, IGoogleService googleService, IAuthCookieService authCookieService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHandlerService = passwordHandlerService ?? throw new ArgumentNullException(nameof(passwordHandlerService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _googleService = googleService ?? throw new ArgumentNullException(nameof(googleService));
            _authCookieService = authCookieService ?? throw new ArgumentNullException(nameof(authCookieService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private static LoginResponse MapUser(User user)
        {
            return new LoginResponse
            {
                Id = user.Id,
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

            return new ApiResponse<string> { Success = true, Message = "verify success" };
        }



        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            var user = await findUserByEmail(request.Email);

            if (user == null)
            {
                throw new KeyNotFoundException("Email not found");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new Exception("Account is not active");
            }

            var passwordVerificationResult = _passwordHandlerService.VerifyPassword(user.PasswordHash, request.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            user.RefreshToken = await _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set refresh token expiry time
            await _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var jwtToken = await _tokenService.GenerateJwtToken(user);
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
            var userId = _tokenService.GetUserId();

            if (userId is 0)
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

            var jwtToken = await _tokenService.GenerateJwtToken(user);
            user.RefreshToken = await _tokenService.GenerateRefreshToken();
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

        public async Task<ApiResponse<LoginResponse>> GoogleLogin(
    GoogleLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Google ID token is required"
                };
            }

            try
            {
                Console.WriteLine("[GoogleLogin] Start");

                var googleClientId = _configuration["Google:ClientId"];

                if (string.IsNullOrWhiteSpace(googleClientId))
                {
                    Console.WriteLine(
                        "[GoogleLogin] Google:ClientId is missing"
                    );

                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Google client ID is not configured"
                    };
                }

                GoogleJsonWebSignature.Payload payload;

                try
                {
                    Console.WriteLine(
                        "[GoogleLogin] Validating Google token"
                    );

                    payload = await GoogleJsonWebSignature.ValidateAsync(
                        request.IdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new[]
                            {
                        googleClientId
                            }
                        });

                    Console.WriteLine(
                        $"[GoogleLogin] Google token valid: {payload.Email}"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[GoogleLogin] Google token validation failed: {ex}"
                    );

                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid Google token"
                    };
                }

                if (string.IsNullOrWhiteSpace(payload.Email))
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Google account does not contain a valid email"
                    };
                }

                if (payload.EmailVerified != true)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Google email is not verified"
                    };
                }

                var email = payload.Email.Trim().ToLowerInvariant();

                Console.WriteLine(
                    $"[GoogleLogin] Finding user by email: {email}"
                );

                var user = await findUserByEmail(email);

                if (user == null)
                {
                    Console.WriteLine(
                        "[GoogleLogin] Creating new user"
                    );

                    var username = string.IsNullOrWhiteSpace(payload.Name)
                        ? email.Split('@')[0]
                        : payload.Name.Trim();

                    user = new User
                    {
                        Username = username,
                        Email = email,
                        PasswordHash = _passwordHandlerService.HashPassword(
                            Guid.NewGuid().ToString()
                        ),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = UserStatus.Active
                    };

                    await _unitOfWork.UserRepository.AddAsync(user);

                    Console.WriteLine(
                        "[GoogleLogin] Saving new user"
                    );

                    await _unitOfWork.SaveChangesAsync();

                    Console.WriteLine(
                        $"[GoogleLogin] New user created. ID: {user.Id}"
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"[GoogleLogin] Existing user found. ID: {user.Id}"
                    );
                }

                if (user.Status != UserStatus.Active)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Account is not active, please contact support."
                    };
                }

                Console.WriteLine(
                    "[GoogleLogin] Generating access token"
                );

                var accessToken =
                    await _tokenService.GenerateJwtToken(user);

                Console.WriteLine(
                    "[GoogleLogin] Generating refresh token"
                );

                var refreshToken =
                    await _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime =
                    DateTime.UtcNow.AddDays(7);
                user.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine(
                    "[GoogleLogin] Saving refresh token"
                );

                await _unitOfWork.SaveChangesAsync();

                Console.WriteLine(
                    "[GoogleLogin] Refresh token saved"
                );

                // Map trước khi ghi cookie để tránh cookie được ghi
                // nhưng response tiếp tục lỗi ở bước mapping.
                Console.WriteLine(
                    "[GoogleLogin] Mapping user"
                );

                var loginResponse = MapUser(user);

                Console.WriteLine(
                    "[GoogleLogin] Setting cookies"
                );

                _authCookieService.SetAuthCookies(
                    accessToken,
                    refreshToken
                );

                Console.WriteLine(
                    "[GoogleLogin] Cookies set"
                );

                Console.WriteLine(
                    "[GoogleLogin] Success"
                );

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Login with Google successful",
                    Data = loginResponse
                };
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(
                    "[GoogleLogin] Database error:"
                );
                Console.WriteLine(ex.ToString());

                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Could not save Google login data"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "[GoogleLogin] Unhandled exception:"
                );
                Console.WriteLine(ex.ToString());

                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Google login failed"
                };
            }
        }
    }
}