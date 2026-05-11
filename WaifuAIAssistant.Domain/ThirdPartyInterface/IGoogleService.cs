using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Domain.Services
{
    public interface IGoogleService
    {
        Task SendEmailAsync(string email, string subject, string body);

        Task SendOtpAsync(string email);

        Task ResendOtpAsync(string email);

        Task<bool> VerifyOtpAsync(string email, string otp);
    }
}
