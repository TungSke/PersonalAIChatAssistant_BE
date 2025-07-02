using Microsoft.AspNetCore.Identity;

namespace WaifuAIAssistant.Domain.Services
{
    public interface IPasswordHandlerService
    {
        string HashPassword(string password);
        PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword);
    }
}
