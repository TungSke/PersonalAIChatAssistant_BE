using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Domain.Services
{
    public interface IGoogleService
    {
        Task SendEmail(string email, string subject, string body);
        Task SendEmailWithOTP(string email, string subject);
        Task<string> GenerateOtp();
        Task<bool> VerifyOtp(string email, string otp);
    }
}
