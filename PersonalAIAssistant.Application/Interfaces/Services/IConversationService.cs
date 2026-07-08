using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Application.DTOs.Response;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Application.Interfaces.Services
{
    public interface IConversationService
    {
        Task<ApiResponse<List<ConversationResponse>>> GetConversationAsync();
        Task<ApiResponse<ConversationResponse>> CreateConversation(ConversationRequest request);
        Task<ApiResponse<ConversationResponse>> DeleteConversation(int id);
    }
}
