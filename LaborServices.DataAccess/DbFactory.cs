using LaborServices.Entity;
using LaborServices.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.DataAccess
{
     public class DbFactory : Disposable, IDbFactory
    {
        LaborServicesDbContext dbContext;

        public LaborServicesDbContext Init()
        {
            return dbContext ?? (dbContext = new LaborServicesDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
            base.DisposeCore();
        }
    }
}
