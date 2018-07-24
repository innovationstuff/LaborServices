using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Model;

namespace LaborServices.Managers
{
    public class SettingStoreBase
    {
        public DbContext Context { get; private set; }
        public DbSet<Setting> DbEntitySet { get; private set; }

        public IQueryable<Setting> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }


        //public IQueryable<Setting> EntitySet => this.DbEntitySet;


        public SettingStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<Setting>();
        }


        public Setting Create(Setting entity)
        {
            entity = this.DbEntitySet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        public void CreateBulk(List<Setting> entites)
        {
            this.DbEntitySet.AddRange(entites);
            Context.SaveChanges();
        }


        public bool Delete(Setting entity)
        {
            this.DbEntitySet.Remove(entity);
            return Context.SaveChanges() > 0;
        }


        public virtual Task<Setting> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual Setting GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual Setting Update(Setting entity)
        {
            this.Context.Entry<Setting>(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return GetById(entity.SettingId);
        }

        public virtual Setting GetByName(string name)
        {
            return this.DbEntitySet.FirstOrDefault(s => s.SettingName.ToLower() == name.ToLower());
        }

        public virtual string GetSettingValueByName(string name)
        {
            var setting = this.DbEntitySet.FirstOrDefault(s => s.SettingName.ToLower() == name.ToLower());
            if (setting != null)
            {
                return setting.SettingValue;
            }
            else
            {
                return string.Empty;
            }
              
        }
    }
}
