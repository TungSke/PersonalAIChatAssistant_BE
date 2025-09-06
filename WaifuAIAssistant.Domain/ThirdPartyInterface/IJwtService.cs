using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Domain.ThirdPartyInterface
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(Users user);
        int? GetUserIdFromJwt(string jwtToken);
        Task<string> GenerateRefreshToken();
    }
}
