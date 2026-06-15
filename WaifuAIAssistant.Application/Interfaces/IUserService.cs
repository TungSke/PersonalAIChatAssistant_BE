using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Service.DTOs.Request;
using WaifuAIAssistant.Service.DTOs.Response;

namespace WaifuAIAssistant.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<RegisterResponse>> Register(RegisterRequest request);

        Task<ApiResponse<LoginResponse>> Login(LoginRequest request);

        Task<ApiResponse<LoginResponse>> Me();

        Task<ApiResponse<string>> VerifyAccount(VerifyAccountRequest request);

        Task<ApiResponse<string>> RefreshToken();

        Task<ApiResponse<string>> Logout();
    }
}
