namespace PersonalAIAssistant.Domain.Interfaces
{
    public interface IAuthCookieService
    {
        void SetAuthCookies(string accessToken, string refreshToken);
        void ClearAuthCookies();
        string? GetRefreshToken();
    }
}