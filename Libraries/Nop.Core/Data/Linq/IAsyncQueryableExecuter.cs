using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Core.Data.Linq
{
    /// <summary>
    /// This interface is intended to be used by Nop.
    /// </summary>
    public interface IAsyncQueryableExecuter
    {
        Task<int> CountAsync<T>(IQueryable<T> queryable);

        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable);
    }
}
