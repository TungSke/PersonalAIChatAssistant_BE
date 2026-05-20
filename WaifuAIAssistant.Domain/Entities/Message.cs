namespace WaifuAIAssistant.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int? UserId { get; set; }
        public int? ModelCharacterId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Conversation Conversation { get; set; } 
        public virtual User? Users { get; set; }
        public virtual ModelsCharacter? ModelsCharacter { get; set; }
    }
}
