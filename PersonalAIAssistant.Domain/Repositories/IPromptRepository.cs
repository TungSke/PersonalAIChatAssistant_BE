namespace PersonalAIAssistant.Domain.Repositories
{
    public interface IPromptRepository
    {
        Task<string> getPromptValueByName(string promptKey);
    }
}
