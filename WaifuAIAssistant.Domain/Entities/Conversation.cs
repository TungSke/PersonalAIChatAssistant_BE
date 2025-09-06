namespace WaifuAIAssistant.Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WaifuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Users User { get; set; } = null!;
        public virtual ModelsCharacter? Waifu { get; set; } = null!;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();    
    }
}
