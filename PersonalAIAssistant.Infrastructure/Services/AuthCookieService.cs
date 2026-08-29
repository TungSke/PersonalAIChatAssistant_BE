using Microsoft.AspNetCore.Http;
using PersonalAIAssistant.Application.Interfaces.Infrastructure;

namespace PersonalAIAssistant.Infrastructure.Services
{
    public class AuthCookieService : IAuthCookieService
    {
        private const string AccessTokenCookieName = "PersonalAI_access_token";
        private const string RefreshTokenCookieName = "PersonalAI_refresh_token";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthCookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void SetAuthCookies(string accessToken, string refreshToken)
        {
            var response = GetResponse();

            response.Cookies.Append(AccessTokenCookieName, accessToken, BuildCookieOptions(TimeSpan.FromHours(1)));
            response.Cookies.Append(RefreshTokenCookieName, refreshToken, BuildCookieOptions(TimeSpan.FromDays(7)));
        }

        public void ClearAuthCookies()
        {
            var response = GetResponse();

            var expired = BuildCookieOptions(TimeSpan.Zero);
            expired.Expires = DateTimeOffset.UtcNow.AddDays(-1);

            response.Cookies.Append(AccessTokenCookieName, string.Empty, expired);
            response.Cookies.Append(RefreshTokenCookieName, string.Empty, expired);
        }

        public string? GetRefreshToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenCookieName];
        }

        private HttpResponse GetResponse()
        {
            return _httpContextAccessor.HttpContext?.Response
                ?? throw new InvalidOperationException("HTTP response is not available.");
        }

        private static CookieOptions BuildCookieOptions(TimeSpan lifetime)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                  
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.Add(lifetime),
                Path = "/",
                IsEssential = true
            };
        }
    }
}