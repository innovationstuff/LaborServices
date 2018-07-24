using LaborServices.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.DataAccess
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        T Add(T entity);

        IEnumerable<T> AddRange(IEnumerable<T> entites);

        void Delete(object id);

        void Delete(T entity);

        void Activate(int id);

        void Deactivate(int id);

        void Update(T entity);

        // ================================================ //

        IQueryable<T> List { get; }

        T Single(int id);

        T Single(Expression<Func<T, bool>> predicate);

        T Single(int id, Expression<Func<T, object>>[] includeProperties);


        T Single(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] includeProperties);
      

        IQueryable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetBySqlQuery(string query, params object[] parameters);

    }
}
