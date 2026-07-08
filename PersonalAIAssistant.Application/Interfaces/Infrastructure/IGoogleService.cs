namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface IGoogleService
    {
        Task SendEmailAsync(string email, string subject, string body);

        Task SendOtpAsync(string email);

        Task ResendOtpAsync(string email);

        Task<bool> VerifyOtpAsync(string email, string otp);
    }
}
