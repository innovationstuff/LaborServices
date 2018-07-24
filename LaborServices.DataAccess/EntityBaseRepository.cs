using LaborServices.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.DataAccess
{
    class EntityBaseRepository<T> : IEntityBaseRepository<T>
       where T : class, IEntityBase, new()
    {


        #region Properties and Attributes

        private LaborServicesDbContext dbContext;

        protected virtual IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected virtual LaborServicesDbContext DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }
        public EntityBaseRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion


        public virtual T Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Added;
            return DbContext.Set<T>().Add(entity);
        }

        public virtual IEnumerable<T> AddRange(IEnumerable<T> entites)
        {
            return DbContext.Set<T>().AddRange(entites);
        }

        public virtual void Delete(object id)
        {
            T entitiyToDelete = DbContext.Set<T>().Find(id);
            if (entitiyToDelete != null)
                Delete(entitiyToDelete);
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            var dbSet = DbContext.Set<T>();
            if (dbEntityEntry.State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void Activate(int id)
        {
            var entity = new T() { Id = id, IsActivated = true };
            var dbSet = DbContext.Set<T>();
            dbSet.Attach(entity);
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Entry<T>(entity).Property(a => a.IsActivated).IsModified = true;
            DbContext.SaveChanges();
        }

        public virtual void Deactivate(int id)
        {
            var entity = new T() { Id = id, IsActivated = false };
            var dbSet = DbContext.Set<T>();
            dbSet.Attach(entity);
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Entry<T>(entity).Property(a => a.IsActivated).IsModified = true;
            DbContext.SaveChanges();
        }
        public virtual void Update(T entity)
        {
            DbContext.Set<T>().Attach(entity);
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        // ================================================ //

        public virtual IQueryable<T> List
        {
            get
            {
                return GetAll();
            }
        }

        public virtual T Single(int id)
        {
            return DbContext.Set<T>().Find(id);
        }

        public virtual T Single(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().FirstOrDefault(predicate);
        }

        public virtual T Single(int id,  Expression<Func<T, object>>[] includeProperties)
        {
            return Find(m => m.Id == id, includeProperties).FirstOrDefault();
        }

        public virtual T Single(Expression<Func<T, bool>> predicate,  Expression<Func<T, object>>[] includeProperties)
        {
            return Find(predicate, includeProperties).FirstOrDefault();

        }


        public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbContext.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            return Find(null, includeProperties);
        }

        public virtual IQueryable<T> GetBySqlQuery(string query, params object[] parameters)
        {
            return DbContext.Set<T>().SqlQuery(query, parameters).AsQueryable<T>();
        }


    }
}
