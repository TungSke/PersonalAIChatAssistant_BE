using Microsoft.AspNetCore.Identity;

namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface IPasswordHandlerService
    {
        string HashPassword(string password);
        PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword);
    }
}
