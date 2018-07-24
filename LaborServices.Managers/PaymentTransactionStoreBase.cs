using LaborServices.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
    public class PaymentTransactionStoreBase
    {
        public DbContext Context { get; private set; }

        public DbSet<PaymentTransaction> DbEntitySet { get; set; }

        public IQueryable<PaymentTransaction> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

        //public IQueryable<PaymentTransaction> EntitySet => this.DbEntitySet;

        public PaymentTransactionStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<PaymentTransaction>();
        }
        public PaymentTransaction Create(PaymentTransaction entity)
        {
            entity = this.DbEntitySet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        public void CreateBulk(List<PaymentTransaction> entites)
        {
            this.DbEntitySet.AddRange(entites);
            Context.SaveChanges();
        }


        public bool Delete(PaymentTransaction entity)
        {
            this.DbEntitySet.Remove(entity);
            return Context.SaveChanges() > 0;
        }


        public virtual Task<PaymentTransaction> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual PaymentTransaction GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual PaymentTransaction Update(PaymentTransaction entity)
        {
            this.Context.Entry<PaymentTransaction>(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return GetById(entity.Id);
        }
    }
}
