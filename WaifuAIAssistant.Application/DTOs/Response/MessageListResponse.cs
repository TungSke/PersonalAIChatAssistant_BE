namespace WaifuAIAssistant.Application.DTOs.Response
{
    public class MessageListResponse
    {
        public List<MessageResponse> Messages { get; set; } = new();
        public long? FirstMessageId { get; set; }
        public long? LastMessageId { get; set; }
        public bool HasMore { get; set; }
    }
}
