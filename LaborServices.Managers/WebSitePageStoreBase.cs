using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Model;

namespace LaborServices.Managers
{
    public class WebSitePageStoreBase
    {
        public DbContext Context { get; private set; }
        public DbSet<WebSitePage> DbEntitySet { get; private set; }

        public IQueryable<WebSitePage> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }


        //public IQueryable<WebSitePage> EntitySet => this.DbEntitySet;


        public WebSitePageStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<WebSitePage>();
        }


        public WebSitePage Create(WebSitePage entity)
        {
            entity = this.DbEntitySet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        public void CreateBulk(List<WebSitePage> entites)
        {
            this.DbEntitySet.AddRange(entites);
            Context.SaveChanges();
        }


        public bool Delete(WebSitePage entity)
        {
            this.DbEntitySet.Remove(entity);
            return Context.SaveChanges() > 0;
        }


        public virtual Task<WebSitePage> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual WebSitePage GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual WebSitePage Update(WebSitePage entity)
        {
            this.Context.Entry<WebSitePage>(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return GetById(entity.PageId);
        }
    }
}
