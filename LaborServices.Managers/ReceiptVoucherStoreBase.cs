using LaborServices.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
    public class ReceiptVoucherStoreBase
    {
        public DbContext Context { get; private set; }

        public DbSet<ReceiptVoucher> DbEntitySet { get; set; }

        public IQueryable<ReceiptVoucher> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }

        //public IQueryable<ReceiptVoucher> EntitySet => this.DbEntitySet;

        public ReceiptVoucherStoreBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<ReceiptVoucher>();
        }
        public ReceiptVoucher Create(ReceiptVoucher entity)
        {
            entity = this.DbEntitySet.Add(entity);
            Context.SaveChanges();
            return entity;
        }


        public void CreateBulk(List<ReceiptVoucher> entites)
        {
            this.DbEntitySet.AddRange(entites);
            Context.SaveChanges();
        }


        public bool Delete(ReceiptVoucher entity)
        {
            this.DbEntitySet.Remove(entity);
            return Context.SaveChanges() > 0;
        }


        public virtual Task<ReceiptVoucher> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual ReceiptVoucher GetById(object id)
        {
            return this.DbEntitySet.Find(new object[] { id });
        }


        public virtual ReceiptVoucher Update(ReceiptVoucher entity)
        {
            this.Context.Entry<ReceiptVoucher>(entity).State = EntityState.Modified;
            Context.SaveChanges();
            return GetById(entity.Id);
        }
    }
}
