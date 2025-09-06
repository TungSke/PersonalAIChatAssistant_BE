using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.InterfaceRepository;

namespace WaifuAIAssistant.Infrastructure.Repository
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context)
        {
        }
        // Additional methods specific to Conversation can be added here
    }
}
