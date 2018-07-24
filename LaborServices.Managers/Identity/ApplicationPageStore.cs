using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Model.Identity;

namespace LaborServices.Managers.Identity
{
    public class ApplicationPageStore : IDisposable
    {
        private bool _disposed;
        private PageStoreBase _pageStore;


        public ApplicationPageStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.Context = context;
            this._pageStore = new PageStoreBase(context);
        }


        public IQueryable<ApplicationPage> Pages
        {
            get
            {
                return this._pageStore.EntitySet;
            }
        }

        public DbContext Context
        {
            get;
            private set;
        }


        public virtual void Create(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Create(page);
            this.Context.SaveChanges();
        }


        public virtual async Task<bool> CreateAsync(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Create(page);
            return await this.Context.SaveChangesAsync() > 0;
        }

        public virtual bool CreateBulk(List<ApplicationPage> pages)
        {
            this.ThrowIfDisposed();
            if (pages.Any() == false)
            {
                throw new ArgumentNullException("pages");
            }
            this._pageStore.CreateBulk(pages);
            return this.Context.SaveChanges() > 0;
        }


        public virtual async Task<bool> CreateBulkAsync(List<ApplicationPage> pages)
        {
            this.ThrowIfDisposed();
            if (pages.Any() == false)
            {
                throw new ArgumentNullException("pages");
            }
            this._pageStore.CreateBulk(pages);
            return await this.Context.SaveChangesAsync() > 0;
        }


        public virtual async Task DeleteAsync(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Delete(page);
            await this.Context.SaveChangesAsync();
        }


        public virtual void Delete(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Delete(page);
            this.Context.SaveChanges();
        }


        public Task<ApplicationPage> FindByIdAsync(long pageId)
        {
            this.ThrowIfDisposed();
            return this._pageStore.GetByIdAsync(pageId);
        }


        public ApplicationPage FindById(long pageId)
        {
            this.ThrowIfDisposed();
            return this._pageStore.GetById(pageId);
        }


        public ApplicationPage FindByName(string groupName)
        {
            this.ThrowIfDisposed();
            return QueryableExtensions
                .FirstOrDefaultAsync<ApplicationPage>(this._pageStore.EntitySet,
                    (ApplicationPage u) => u.NameEn.ToUpper() == groupName.ToUpper() || u.NameAr.ToUpper() == groupName.ToUpper()).Result;
        }

        public Task<ApplicationPage> FindByNameAsync(string groupName)
        {
            this.ThrowIfDisposed();
            return QueryableExtensions
                .FirstOrDefaultAsync<ApplicationPage>(this._pageStore.EntitySet,
                    (ApplicationPage u) => u.NameEn.ToUpper() == groupName.ToUpper() || u.NameAr.ToUpper() == groupName.ToUpper());
        }


        public virtual async Task UpdateAsync(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Update(page);
            await this.Context.SaveChangesAsync();
        }


        public virtual void Update(ApplicationPage page)
        {
            this.ThrowIfDisposed();
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            this._pageStore.Update(page);
            this.Context.SaveChanges();
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
            this._pageStore = null;
        }
    }
}
