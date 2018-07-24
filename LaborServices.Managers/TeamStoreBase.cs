using LaborServices.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
	public class TeamStoreBase
	{
		public DbContext Context { get; private set; }

		public DbSet<Team> DbEntitySet { get; set; }

        public IQueryable<Team> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

		//public IQueryable<Team> EntitySet => this.DbEntitySet;

		public TeamStoreBase(DbContext context)
		{
			this.Context = context;
			this.DbEntitySet = context.Set<Team>();
		}
		public Team Create(Team entity)
		{
			entity = this.DbEntitySet.Add(entity);
			Context.SaveChanges();
			return entity;
		}


		public void CreateBulk(List<Team> entites)
		{
			this.DbEntitySet.AddRange(entites);
			Context.SaveChanges();
		}


		public bool Delete(Team entity)
		{
			this.DbEntitySet.Remove(entity);
			return Context.SaveChanges() > 0;
		}


		public virtual Task<Team> GetByIdAsync(object id)
		{
			return this.DbEntitySet.FindAsync(new object[] { id });
		}


		public virtual Team GetById(object id)
		{
			return this.DbEntitySet.Find(new object[] { id });
		}


		public virtual Team Update(Team entity)
		{
			this.Context.Entry<Team>(entity).State = EntityState.Modified;
			Context.SaveChanges();
			return GetById(entity.Id);
		}
	}
}
