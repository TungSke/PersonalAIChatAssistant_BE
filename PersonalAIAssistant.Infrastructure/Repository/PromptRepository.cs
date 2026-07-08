using Microsoft.EntityFrameworkCore;
using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Repositories;

namespace PersonalAIAssistant.Infrastructure.Repository
{
    public class PromptRepository : Repository<PromptTemplate>,IPromptRepository
    {
        public PromptRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<string> getPromptValueByName(string promptKey)
        {
            var prompt = await GetAll().FirstOrDefaultAsync(p => p.PromptKey == promptKey);
            if (prompt == null)
            {
                throw new KeyNotFoundException($"Prompt with key '{promptKey}' not found.");
            }
            return prompt.Content;
        }
    }
}
