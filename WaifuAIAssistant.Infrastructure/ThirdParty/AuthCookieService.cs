using Microsoft.AspNetCore.Http;
using WaifuAIAssistant.Domain.Interfaces;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class AuthCookieService : IAuthCookieService
    {
        private const string AccessTokenCookieName = "waifu_access_token";
        private const string RefreshTokenCookieName = "waifu_refresh_token";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthCookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void SetAuthCookies(string accessToken, string refreshToken)
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response == null)
            {
                throw new InvalidOperationException("HTTP response is not available.");
            }

            response.Cookies.Append(AccessTokenCookieName, accessToken, BuildCookieOptions(TimeSpan.FromHours(1)));
            response.Cookies.Append(RefreshTokenCookieName, refreshToken, BuildCookieOptions(TimeSpan.FromDays(7)));
        }

        public void ClearAuthCookies()
        {
            var response = _httpContextAccessor.HttpContext?.Response;
            if (response == null)
            {
                return;
            }

            var expired = new CookieOptions
            {
                HttpOnly = true,
                Secure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? true,
                SameSite = (_httpContextAccessor.HttpContext?.Request.IsHttps ?? true) ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "/"
            };

            response.Cookies.Append(AccessTokenCookieName, string.Empty, expired);
            response.Cookies.Append(RefreshTokenCookieName, string.Empty, expired);
        }

        public string? GetRefreshToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenCookieName];
        }

        private CookieOptions BuildCookieOptions(TimeSpan lifetime)
        {
            var isHttps = _httpContextAccessor.HttpContext?.Request.IsHttps ?? true;

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.Add(lifetime),
                Path = "/"
            };
        }
    }
}