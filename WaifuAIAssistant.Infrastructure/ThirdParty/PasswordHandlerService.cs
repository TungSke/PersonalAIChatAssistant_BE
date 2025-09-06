using Microsoft.AspNetCore.Identity;
using WaifuAIAssistant.Domain.Services;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class PasswordHandlerService : IPasswordHandlerService
    {
        private readonly PasswordHasher<string> _passwordHasher;

        public PasswordHandlerService()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }
            return _passwordHasher.HashPassword(null, password);
        }

        public PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword), "Hashed password cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(providedPassword))
            {
                throw new ArgumentNullException(nameof(providedPassword), "Provided password cannot be null or empty.");
            }
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
        }
    }
}
