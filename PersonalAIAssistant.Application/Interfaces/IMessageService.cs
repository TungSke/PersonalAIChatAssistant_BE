using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Domain.Base;

namespace PersonalAIAssistant.Application.Interfaces
{
    public interface IMessageService
    {
        Task<ApiResponse<MessageListResponse>> GetMessagesFromConversation(
            int conversationId,
            int limit = 30,
            long? beforeMessageId = null);
        Task<ApiResponse<MessageResponse>> CreateMessage(MessageRequest request);
        Task<ApiResponse<string>> DeleteMessage(int messageId);
    }
}
