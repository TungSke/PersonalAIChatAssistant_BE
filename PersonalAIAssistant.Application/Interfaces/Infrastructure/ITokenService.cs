using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
        Task<int> GetUserId();
        Task<string> GenerateRefreshToken();
    }
}
