using LaborServices.DataAccess;
using LaborServices.Utility;

namespace LaborServices.Managers
{
    public class EntityBaseManager : Disposable
    {
        protected UnitOfWork unitOfWork;
        public EntityBaseManager()
        {
            unitOfWork = new UnitOfWork(new DbFactory());
        }
        protected override void DisposeCore()
        {
            unitOfWork.Dispose();
            base.DisposeCore();
        }
    }
}
