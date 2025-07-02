using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaifuAIAssistant.Domain
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        IQueryable<T> GetAll();

        Task FindAsync(params object[] keyValues);

        Task Remove(T entity);

        Task RemoveRange(IEnumerable<T> entities);

        Task Update(T entity);

        Task UpdateRange(IEnumerable<T> entities);
    }
}
