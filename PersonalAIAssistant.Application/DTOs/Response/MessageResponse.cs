namespace PersonalAIAssistant.Application.DTOs.Response
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string role { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
