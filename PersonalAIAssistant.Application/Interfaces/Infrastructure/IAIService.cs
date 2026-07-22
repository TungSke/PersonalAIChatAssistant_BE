using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Application.Interfaces.Infrastructure
{
    public interface IAIService
    {
        Task<string> GenerateReply(
            Conversation conversation,
            ModelsCharacter character,
            List<Message> recentMessages,
            string newUserMessage
        );

        Task<string> SummarizeConversation(
    string? currentSummary,
    List<Message> recentMessages);
    }
}
