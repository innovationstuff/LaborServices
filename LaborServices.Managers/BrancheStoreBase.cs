using LaborServices.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
	public class BrancheStoreBase
	{
		public DbContext Context { get; private set; }

		public DbSet<Branche> DbEntitySet { get; set; }

        public IQueryable<Branche> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

		//public IQueryable<Branche> EntitySet => this.DbEntitySet;

		public BrancheStoreBase(DbContext context)
		{
			this.Context = context;
			this.DbEntitySet = context.Set<Branche>();
		}
		public Branche Create(Branche entity)
		{
			entity = this.DbEntitySet.Add(entity);
			Context.SaveChanges();
			return entity;
		}


		public void CreateBulk(List<Branche> entites)
		{
			this.DbEntitySet.AddRange(entites);
			Context.SaveChanges();
		}


		public bool Delete(Branche entity)
		{
			this.DbEntitySet.Remove(entity);
			return Context.SaveChanges() > 0;
		}


		public virtual Task<Branche> GetByIdAsync(object id)
		{
			return this.DbEntitySet.FindAsync(new object[] { id });
		}


		public virtual Branche GetById(object id)
		{
			return this.DbEntitySet.Find(new object[] { id });
		}


		public virtual Branche Update(Branche entity)
		{
			this.Context.Entry<Branche>(entity).State = EntityState.Modified;
			Context.SaveChanges();
			return GetById(entity.Id);
		}
	}
}
