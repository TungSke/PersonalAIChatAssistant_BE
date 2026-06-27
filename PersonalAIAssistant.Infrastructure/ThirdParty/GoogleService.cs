using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using PersonalAIAssistant.Domain.Services;
using PersonalAIAssistant.Domain.ThirdPartyInterface;

namespace PersonalAIAssistant.Infrastructure.ThirdParty
{
    public class GoogleService : IGoogleService
    {
        private readonly IRedisCacheService _cache;
        private readonly IConfiguration _configuration;

        private const int OTP_EXPIRED_MINUTES = 5;
        private const int RESEND_COOLDOWN_SECONDS = 30;

        public GoogleService(
            IRedisCacheService cache,
            IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var senderEmail = _configuration["EmailSettings:SenderEmail"];

                var senderPassword = _configuration["EmailSettings:Password"];

                if (string.IsNullOrWhiteSpace(senderEmail) ||
                    string.IsNullOrWhiteSpace(senderPassword))
                {
                    throw new InvalidOperationException(
                        "Email configuration is missing.");
                }

                using var mail = new MailMessage
                {
                    From = new MailAddress(
                        senderEmail,
                        "PersonalAI AI Support Team"),

                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,

                    Credentials = new NetworkCredential(
                        senderEmail,
                        senderPassword),

                    EnableSsl = true,

                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to send email: {ex.Message}");
            }
        }

        public async Task SendOtpAsync(string email)
        {
            ValidateEmail(email);

            email = NormalizeEmail(email);

            var cooldownKey = GetCooldownKey(email);

            // Check resend cooldown
            var cooldownExists =
                await _cache.GetAsync<string>(cooldownKey);

            if (!string.IsNullOrWhiteSpace(cooldownExists))
            {
                throw new Exception(
                    "Please wait before requesting another OTP.");
            }

            var otp = GenerateOtp();

            var otpKey = GetOtpKey(email);

            // Save OTP
            await _cache.SetAsync(
                otpKey,
                otp,
                TimeSpan.FromMinutes(OTP_EXPIRED_MINUTES));

            // Save cooldown
            await _cache.SetAsync(
                cooldownKey,
                "1",
                TimeSpan.FromSeconds(RESEND_COOLDOWN_SECONDS));

            var emailBody =
                BuildOtpEmailTemplate(email, otp);

            await SendEmailAsync(
                email,
                "Your OTP for PersonalAI AI Login",
                emailBody);

            Console.WriteLine($"OTP GENERATED: {otp}");
            Console.WriteLine($"OTP KEY: {otpKey}");
        }

        public async Task ResendOtpAsync(string email)
        {
            ValidateEmail(email);

            email = NormalizeEmail(email);

            var otpKey = GetOtpKey(email);

            // Remove old OTP
            await _cache.RemoveAsync(otpKey);

            // Generate new OTP
            await SendOtpAsync(email);
        }

        public async Task<bool> VerifyOtpAsync(
            string email,
            string otp)
        {
            ValidateEmail(email);

            if (string.IsNullOrWhiteSpace(otp))
            {
                return false;
            }

            email = NormalizeEmail(email);

            var otpKey = GetOtpKey(email);

            var cachedOtp =
                await _cache.GetAsync<string>(otpKey);

            Console.WriteLine($"OTP KEY: {otpKey}");
            Console.WriteLine($"CACHED OTP: {cachedOtp}");
            Console.WriteLine($"INPUT OTP: {otp}");

            if (string.IsNullOrWhiteSpace(cachedOtp))
            {
                return false;
            }

            var isValid =
                cachedOtp.Trim() == otp.Trim();

            if (!isValid)
            {
                return false;
            }

            // Remove OTP after verify success
            await _cache.RemoveAsync(otpKey);

            return true;
        }

        private static string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }
        }

        private static string NormalizeEmail(string email)
        {
            return email
                .Trim()
                .ToLowerInvariant();
        }

        private static string GetOtpKey(string email)
        {
            return $"otp:{email}";
        }

        private static string GetCooldownKey(string email)
        {
            return $"otp_cooldown:{email}";
        }

        private static string BuildOtpEmailTemplate(
            string email,
            string otp)
        {
            return $@"
                <div style='font-family: Arial, sans-serif'>
                    <h2>PersonalAI AI Login Verification</h2>

                    <p>Hello {email},</p>

                    <p>
                        You requested an OTP to login to your account.
                    </p>

                    <p>Your OTP code is:</p>

                    <h1 style='color: #4F46E5'>
                        {otp}
                    </h1>

                    <p>
                        This OTP will expire in
                        {OTP_EXPIRED_MINUTES} minutes.
                    </p>

                    <p>
                        If you did not request this OTP,
                        please ignore this email.
                    </p>

                    <br />

                    <p>Best regards,</p>

                    <p>
                        PersonalAI AI Support Team
                    </p>
                </div>";
        }
    }
}