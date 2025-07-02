using Microsoft.EntityFrameworkCore;
using WaifuAIAssistant.Infrastructure;

namespace WaifuAIAssistant.Domain.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public async Task Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task FindAsync(params object[] keyValues)
        {
            await _context.Set<T>().FindAsync(keyValues);
        }

        public IQueryable<T> GetAll() => _context.Set<T>();

        public async Task Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }
    }
}
