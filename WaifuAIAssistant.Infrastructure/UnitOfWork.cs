using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.InterfaceRepository;
using WaifuAIAssistant.Infrastructure.Repository;

namespace WaifuAIAssistant.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _context;

        private IUserRepository _userRepository;
        private ICharacterEmotionsRepository _characterEmotionsRepository;
        private IConversationRepository _conversationRepository;
        private IMessageRepository _messageRepository;
        private IModelsCharacterRepository _modelRepository;
        private IPromptRepository _promptRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public ICharacterEmotionsRepository CharacterEmotionsRepository => _characterEmotionsRepository ??= new CharacterEmotionRepository(_context);

        public IConversationRepository ConversationRepository => _conversationRepository ??= new ConversationRepository(_context);

        public IMessageRepository MessageRepository => _messageRepository ??= new MessageRepository(_context);

        public IModelsCharacterRepository ModelRepository => _modelRepository ??= new ModelsCharacterRepository(_context);

        public IPromptRepository PromptRepository => _promptRepository ??= new PromptRepository(_context);

        public Task BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }
        public Task CommitTransactionAsync()
        {
            return _context.Database.CommitTransactionAsync();
        }
        public Task RollbackTransactionAsync()
        {
            return _context.Database.RollbackTransactionAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
