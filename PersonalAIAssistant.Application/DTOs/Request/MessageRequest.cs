using Swashbuckle.AspNetCore.Annotations;

namespace PersonalAIAssistant.Application.DTOs.Request
{
    public class MessageRequest
    {
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
