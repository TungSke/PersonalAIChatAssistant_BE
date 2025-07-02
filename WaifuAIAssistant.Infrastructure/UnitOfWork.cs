using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Interfaces;
using WaifuAIAssistant.Infrastructure.Repository;

namespace WaifuAIAssistant.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _context;

        private IUserRepository _userRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

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
