using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Domain.Base;
using WaifuAIAssistant.Domain.Entities;
using WaifuAIAssistant.Domain.InterfaceRepository;

namespace WaifuAIAssistant.Infrastructure.Repository
{
    public class ModelsCharacterRepository : Repository<ModelsCharacter>, IModelsCharacterRepository
    {
        public ModelsCharacterRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<ModelsCharacter?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.ModelsCharacters.FirstOrDefaultAsync(m => m.Name == name, cancellationToken);
        }
        public async Task<IEnumerable<ModelsCharacter>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ModelsCharacters
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
