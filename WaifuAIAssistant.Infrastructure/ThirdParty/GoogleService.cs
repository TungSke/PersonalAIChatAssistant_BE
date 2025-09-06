using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class GoogleService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public GoogleService(IMemoryCache cache)
        {
            _cache = cache;

        }
        public async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                var emailSender = _configuration["EmailSettings:SenderEmail"];
                var emailSenderPassword = _configuration["EmailSettings:Password"];

                if (string.IsNullOrEmpty(emailSender) || string.IsNullOrEmpty(emailSenderPassword))
                {
                    throw new Exception("Email sender credentials are not configured.");
                }

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("support@nutridiet.com", "NutriDiet Support Team"),
                    Subject = subject,
                    Body = body ?? "No content available",
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailSender, emailSenderPassword);
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when sending email: " + ex.Message);
            }
        }


        public async Task SendEmailWithOTP(string email, string subject)
        {
            try
            {
                var otp = GenerateOtp();

                _cache.Set(email, otp, TimeSpan.FromMinutes(5));

                // Cải thiện nội dung email
                var emailContent = $@"
                            <p>Xin chào {email},</p>
                            <p>Bạn nhận được email này vì đã yêu cầu mã OTP để đăng nhập vào tài khoản NutriDiet của mình.</p>
                            <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
                            <p>Mã này có hiệu lực trong 5 phút.</p>
                            <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.</p>
                            <p>Trân trọng,</p>
                            <p>Đội ngũ hỗ trợ NutriDiet</p>
                            <p>Website: https://www.nutridiet.live/</p>
                            ";

                var emailSender = Environment.GetEnvironmentVariable("EMAIL_SENDER");
                var emailSenderPassword = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWORD");

                // Sử dụng tên miền riêng (thay "no-reply@yourdomain.com" bằng tên miền của bạn)
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("support@nutridiet.com", "NutriDiet Support Team"),
                    Subject = subject,
                    Body = emailContent,
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                // Gửi email qua SMTP
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(emailSender, emailSenderPassword);

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when sending email: " + ex.Message);
            }
        }

        public async Task<bool> VerifyOtp(string email, string otp)
        {
            var cachedOtp = _cache.Get(email) as string;

            Console.WriteLine($"Cached OTP: {cachedOtp} for email: {email}");
            Console.WriteLine($"Provided OTP: {otp}");

            if (!string.IsNullOrEmpty(cachedOtp) && cachedOtp.Equals(otp))
            {
                _cache.Remove(email);
                return true;
            }

            return false;
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
