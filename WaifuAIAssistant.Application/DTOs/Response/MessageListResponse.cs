namespace WaifuAIAssistant.Application.DTOs.Response
{
    public class MessageListResponse
    {
        public string? ModelId { get; set; }
        public string? ModelName { get; set; }
        public string? ModelAvatarUrl { get; set; }
        public long? FirstMessageId { get; set; }
        public List<MessageResponse>? Messages { get; set; } = new();
    }
}