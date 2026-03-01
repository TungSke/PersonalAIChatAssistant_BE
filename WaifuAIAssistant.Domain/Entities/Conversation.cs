using WaifuAIAssistant.Domain.Enums;

namespace WaifuAIAssistant.Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WaifuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; } = string.Empty;
        public DateTime SummaryAt { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public required ConversationStatus Status { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ModelsCharacter? Waifu { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();    
    }
}
