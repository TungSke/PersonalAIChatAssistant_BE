using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Domain.ThirdPartyInterface
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(Users user);
        Task<int> GetUserId();
        Task<string> GenerateRefreshToken();
    }
}
