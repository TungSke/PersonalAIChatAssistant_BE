using System;
using System.Collections.Generic;
using System.Text;

namespace WaifuAIAssistant.Domain.InterfaceRepository
{
    public interface IPromptRepository
    {
        Task<string> getPromptValueByName(string promptKey);
    }
}
