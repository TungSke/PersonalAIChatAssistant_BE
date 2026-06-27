using System.ComponentModel.DataAnnotations;

namespace PersonalAIAssistant.Service.DTOs.Request
{
    public class RegisterRequest
    {
        public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long."), Required]
        public required string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
