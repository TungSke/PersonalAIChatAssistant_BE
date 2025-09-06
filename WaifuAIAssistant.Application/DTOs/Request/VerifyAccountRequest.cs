using System.ComponentModel.DataAnnotations;

namespace WaifuAIAssistant.Application.DTOs.Request
{
    public class VerifyAccountRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
