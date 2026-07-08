using PersonalAIAssistant.Domain.Base;
using PersonalAIAssistant.Domain.Entities;
using PersonalAIAssistant.Domain.Repositories;

namespace PersonalAIAssistant.Infrastructure.Repository
{
    public class CharacterEmotionRepository : Repository<CharacterEmotion>, ICharacterEmotionsRepository
    {
        public CharacterEmotionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
