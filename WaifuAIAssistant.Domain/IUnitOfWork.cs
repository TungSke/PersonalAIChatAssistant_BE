using WaifuAIAssistant.Domain.InterfaceRepository;

namespace WaifuAIAssistant.Domain
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        ICharacterEmotionsRepository CharacterEmotionsRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IMessageRepository MessageRepository { get; }
        IModelsCharacterRepository ModelRepository { get; }
        IUserRepository UserRepository { get; }
        IPromptRepository PromptRepository { get; }
    }
}
