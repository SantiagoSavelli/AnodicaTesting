using System.Linq.Expressions;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IRepository<T, TKey> where T : class
    {
        Task<T?> GetAsync(TKey id);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool? NoTracking = false
        );

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool? NoTracking = false
        );

        Task AddAsync(T entity);

        void Remove(T entity);
    }
}