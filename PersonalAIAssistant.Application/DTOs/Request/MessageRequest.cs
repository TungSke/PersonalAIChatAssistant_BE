using System.ComponentModel;

namespace PersonalAIAssistant.Application.DTOs.Request
{
    public class MessageRequest
    {
        public int ConversationId { get; set; }

        [Description("The content of the message to be sent")]
        public string Content { get; set; } = string.Empty;
    }
}
