using LaborServices.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
	public class AboutStoreBase
	{
		public DbContext Context { get; private set; }

		public DbSet<About> DbEntitySet { get; set; }

		public IQueryable<About> EntitySet => this.DbEntitySet;

		public AboutStoreBase(DbContext context)
		{
			this.Context = context;
			this.DbEntitySet = context.Set<About>();
		}
		public About Create(About entity)
		{
			entity = this.DbEntitySet.Add(entity);
			Context.SaveChanges();
			return entity;
		}


		public void CreateBulk(List<About> entites)
		{
			this.DbEntitySet.AddRange(entites);
			Context.SaveChanges();
		}


		public bool Delete(About entity)
		{
			this.DbEntitySet.Remove(entity);
			return Context.SaveChanges() > 0;
		}


		public virtual Task<About> GetByIdAsync(object id)
		{
			return this.DbEntitySet.FindAsync(new object[] { id });
		}


		public virtual About GetById(object id)
		{
			return this.DbEntitySet.Find(new object[] { id });
		}


		public virtual About Update(About entity)
		{
			this.Context.Entry<About>(entity).State = EntityState.Modified;
			Context.SaveChanges();
			return GetById(entity.Id);
		}
	}
}
