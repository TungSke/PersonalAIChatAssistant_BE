using System.ComponentModel.DataAnnotations;

namespace PersonalAIAssistant.Application.DTOs.Request
{
    public class VerifyAccountRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
