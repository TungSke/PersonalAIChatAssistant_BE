using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;

namespace WaifuAIAssistant.Application.Interfaces
{
    public interface IMessageService
    {
        Task<ApiResponse<List<MessageResponse>>> GetMessagesFromConversation(int conversationId);
        Task<ApiResponse<MessageRequest>> CreateMessage(MessageRequest request);
    }
}
