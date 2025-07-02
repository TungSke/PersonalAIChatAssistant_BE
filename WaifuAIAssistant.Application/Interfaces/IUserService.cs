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
    }
}
