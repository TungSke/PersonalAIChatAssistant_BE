using Swashbuckle.AspNetCore.Annotations;

namespace WaifuAIAssistant.Application.DTOs.Request
{
    public class MessageRequest
    {
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
