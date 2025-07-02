using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Domain.Services
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(Users user);
        string? GetUserIdFromJwt(string jwtToken);
        Task<string> GenerateRefreshToken();
    }
}
