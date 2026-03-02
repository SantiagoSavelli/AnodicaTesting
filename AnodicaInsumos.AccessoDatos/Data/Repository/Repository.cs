using AnodicaInsumos.AccessoDatos.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AnodicaInsumos.AccessoDatos.Data.Repository
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T? Get(TKey id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeProperties = null)
        {
            // Se crea una consulta IQueryable a partir del DbSet del contexto
            IQueryable<T> query = dbSet;

            // Se aplica el filtro si se proporciona 
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Se incluyen las propiedades de navegacion
            if (includeProperties != null)
            {
                // Se divide la cadena de propiedades por coma y se itera sobre ellas
                foreach (var includeProperty in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Se aplica el ordenamiento si se proporciona
            if (orderBy != null)
            {
                // Se ejecuta la funcion de ordenamiento y se convierte en una lista
                return orderBy(query).ToList();
            }

            // Si no se proporciona ordenamiento, simplemente se convierte la consulta en una lista
            return query.ToList();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            // Se crea una consulta IQueryable a partir del DbSet del contexto
            IQueryable<T> query = dbSet;

            // Se aplica el filtro si se proporciona 
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Se incluyen las propiedades de navegacion
            if (includeProperties != null)
            {
                // Se divide la cadena de propiedades por coma y se itera sobre ellas
                foreach (var includeProperty in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            // Si no se proporciona ordenamiento, simplemente se convierte la consulta en una lista
            return query.FirstOrDefault();
        }

        public void Remove(TKey id)
        {
            var entityToRemove = dbSet.Find(id);
            if (entityToRemove != null)
                dbSet.Remove(entityToRemove);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
