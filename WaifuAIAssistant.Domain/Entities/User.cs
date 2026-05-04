using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WaifuAIAssistant.Domain.Enums;

namespace WaifuAIAssistant.Domain.Entities
{
    [Index(nameof(Username) , nameof(Email))]
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;

        public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
