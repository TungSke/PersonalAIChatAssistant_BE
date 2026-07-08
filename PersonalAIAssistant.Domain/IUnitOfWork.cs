using PersonalAIAssistant.Domain.Repositories;

namespace PersonalAIAssistant.Domain
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<Task> operation);
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
