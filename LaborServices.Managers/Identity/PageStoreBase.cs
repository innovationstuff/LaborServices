using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Model.Identity;

namespace LaborServices.Managers.Identity
{
   public  class PageStoreBase
    {
        public DbContext Context { get; private set; }
        public DbSet<ApplicationPage> DbEntitySet { get; private set; }


        public IQueryable<ApplicationPage> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }


        public PageStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<ApplicationPage>();
        }


        public void Create(ApplicationPage entity)
        {
            this.DbEntitySet.Add(entity);
        }


        public void CreateBulk(List<ApplicationPage> entites)
        {
            this.DbEntitySet.AddRange(entites);
        }


        public void Delete(ApplicationPage entity)
        {
            this.DbEntitySet.Remove(entity);
        }


        public virtual Task<ApplicationPage> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual ApplicationPage GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual void Update(ApplicationPage entity)
        {
            if (entity != null)
            {
                this.Context.Entry<ApplicationPage>(entity).State = EntityState.Modified;
            }
        }
    }
}
