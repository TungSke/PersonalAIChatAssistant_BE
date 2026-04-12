namespace WaifuAIAssistant.Application.DTOs.Response
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int? UserId { get; set; }
        public int? ModelCharacterId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
