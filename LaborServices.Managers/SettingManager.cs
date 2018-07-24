using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Managers
{
    public class SettingManager : IDisposable
    {
        private bool _disposed;
        private SettingStoreBase _settingStore;


        public SettingManager(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.Context = context;
            this._settingStore = new SettingStoreBase(context);
        }

        public DbContext Context
        {
            get;
            private set;
        }

        public bool IsSMSEnabled()
        {
            var setting = this._settingStore.GetByName("EnableSmS");
            return Convert.ToBoolean(setting.SettingValue);
        }

        // DISPOSE STUFF: ===============================================

        public bool DisposeContext
        {
            get;
            set;
        }


        private void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (this.DisposeContext && disposing && this.Context != null)
            {
                this.Context.Dispose();
            }
            this._disposed = true;
            this.Context = null;
            this._settingStore = null;
        }
    }
}
