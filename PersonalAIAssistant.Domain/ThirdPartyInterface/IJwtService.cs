using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Domain.ThirdPartyInterface
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(User user);
        Task<int> GetUserId();
        Task<string> GenerateRefreshToken();
    }
}
