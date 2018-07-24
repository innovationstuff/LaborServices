using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Model;

namespace LaborServices.Managers
{
    public class SliderStoreBase
    {
        public DbContext Context { get; private set; }
        public DbSet<Slider> DbEntitySet { get; private set; }

        public IQueryable<Slider> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

        //public IQueryable<Slider> EntitySet => this.DbEntitySet;


        public SliderStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<Slider>();
        }


        public Slider Create(Slider entity)
        {
            entity = this.DbEntitySet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        public void CreateBulk(List<Slider> entites)
        {
            this.DbEntitySet.AddRange(entites);
            Context.SaveChanges();
        }


        public bool Delete(Slider entity)
        {
            this.DbEntitySet.Remove(entity);
            return Context.SaveChanges() > 0;
        }


        public virtual Task<Slider> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual Slider GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual Slider Update(Slider entity)
        {
            this.Context.Entry<Slider>(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return GetById(entity.Id);
        }
    }
}
