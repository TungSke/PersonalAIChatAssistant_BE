using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.DTOs.Response;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Application.Interfaces
{
    public interface IConversationService
    {
        Task<ApiResponse<List<ConversationResponse>>> GetConversationAsync();
        Task<ApiResponse<ConversationResponse>> CreateConversation(ConversationRequest request);
        Task<ApiResponse<ConversationResponse>> DeleteConversation(int id);
    }
}
