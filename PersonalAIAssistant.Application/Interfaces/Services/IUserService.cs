using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Service.DTOs.Response;

namespace PersonalAIAssistant.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request);

        Task<ApiResponse<LoginResponse>> Login(LoginRequest request);

        Task<ApiResponse<LoginResponse>> Me();

        Task<ApiResponse<string>> VerifyAccount(VerifyAccountRequest request);

        Task<ApiResponse<string>> RefreshToken();

        Task<ApiResponse<string>> Logout();

        Task<ApiResponse<LoginResponse>> GoogleLogin(string idToken);

        Task<ApiResponse<string>> RegisterWithPhoneNumber(RegisterWithPhoneNumberRequest request);

        Task<ApiResponse<string>> VerifyPhoneNumber(VerifyPhoneNumberRequest request);
    }
}
