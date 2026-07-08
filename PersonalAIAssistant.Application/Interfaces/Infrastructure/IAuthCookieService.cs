namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface IAuthCookieService
    {
        void SetAuthCookies(string accessToken, string refreshToken);
        void ClearAuthCookies();
        string? GetRefreshToken();
    }
}