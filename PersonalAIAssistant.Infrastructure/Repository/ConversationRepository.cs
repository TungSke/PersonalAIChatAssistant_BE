using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Repositories;

namespace PersonalAIAssistant.Infrastructure.Repository
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }
        // Additional methods specific to Conversation can be added here
    }
}
