using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalAIAssistant.Domain.Entities;

namespace PersonalAIAssistant.Domain.ThirdPartyInterface
{
    public interface IGenerationAIService
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
