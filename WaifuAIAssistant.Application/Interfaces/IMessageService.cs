using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;

namespace WaifuAIAssistant.Application.Interfaces
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
