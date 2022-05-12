using System.Linq.Expressions;

namespace KDRC_Core.Repositories;

public interface ICommonMongoRepository<T>
{
    public Task<T?> FindSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    public Task<T?> FindFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    public Task<List<T>> FindMultipleAsync(Expression<Func<T, bool>> predicate);
    public Task<T> CreateOneAsync(T targetAccount);
    public Task<T> UpdateOneAsync(T targetAccount);
    public Task DeleteOneAsync(T targetT);
}