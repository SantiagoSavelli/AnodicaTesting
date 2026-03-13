using System.Linq.Expressions;

namespace AnodicaInsumos.AccessoDatos.Data.Repository.IRepository
{
    public interface IRepository<T, TKey> where T : class
    {
        T? Get(TKey id);
        Task<T?> GetAsync(TKey id);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null
        );

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null
        );

        T? GetFirstOrDefault(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null
        );

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null
        );

        void Add(T entity);
        Task AddAsync(T entity);

        void Remove(TKey id);
        void Remove(T entity);
    }
}