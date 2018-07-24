using LaborServices.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.DataAccess
{
    public interface IUnitOfWork
    {
        IEntityBaseRepository<T> Repository<T>() where T : class, IEntityBase, new();
        int SaveChanges();
    }
}
