using WaifuAIAssistant.Domain.Interfaces;

namespace WaifuAIAssistant.Domain
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Dispose();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IUserRepository UserRepository { get; }
    }
}
