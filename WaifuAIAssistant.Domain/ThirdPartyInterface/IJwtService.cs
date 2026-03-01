using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Domain.ThirdPartyInterface
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(User user);
        Task<int> GetUserId();
        Task<string> GenerateRefreshToken();
    }
}
