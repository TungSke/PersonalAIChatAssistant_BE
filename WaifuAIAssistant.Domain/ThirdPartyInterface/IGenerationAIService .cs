using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaifuAIAssistant.Domain.Entities;

namespace WaifuAIAssistant.Domain.ThirdPartyInterface
{
    public interface IGenerationAIService
    {
        Task<string> Response(int conversationId, ModelsCharacter modelsCharacter, string newUserMessage, int userId);
    }
}
