//using WaifuAIAssistant.Domain;

//namespace WaifuAIAssistant.Infrastructure
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly ApplicationDbContext _context;
//        public UnitOfWork(ApplicationDbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }
//        public Task<int> SaveChangesAsync()
//        {
//            // Implementation for saving changes to the database
//            return _context.SaveChangesAsync();
//        }
//        public Task BeginTransactionAsync()
//        {
//            return _context.Database.BeginTransactionAsync();
//        }
//        public Task CommitTransactionAsync()
//        {
//            // Implementation for committing a transaction
//            throw new NotImplementedException();
//        }
//        public Task RollbackTransactionAsync()
//        {
//            // Implementation for rolling back a transaction
//            throw new NotImplementedException();
//        }
//        public void Dispose()
//        {
//            _context.Dispose();
//        }
//    }
//}
