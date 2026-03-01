using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.InterfaceRepository;

namespace WaifuAIAssistant.Infrastructure.Repository
{
    public class CharacterEmotionRepository : Repository<CharacterEmotion>, ICharacterEmotionsRepository
    {
        public CharacterEmotionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
