using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalAIAssistant.Domain.InterfaceRepository
{
    public interface IPromptRepository
    {
        Task<string> getPromptValueByName(string promptKey);
    }
}
