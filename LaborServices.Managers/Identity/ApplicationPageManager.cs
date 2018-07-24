using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LaborServices.Entity;
using LaborServices.Model.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace LaborServices.Managers.Identity
{
    public class ApplicationPageManager
    {
        private ApplicationPageStore _pageStore;
        private LaborServicesDbContext _db;
        private ApplicationRoleManager _roleManager;
        private string _errorMessage;

        public ApplicationPageManager(IOwinContext context)
        {
            _db = context.Get<LaborServicesDbContext>();
            _roleManager = context.Get<ApplicationRoleManager>();
            _pageStore = new ApplicationPageStore(_db);
            _errorMessage = "";
        }

        public IQueryable<ApplicationPage> Pages
        {
            get
            {
                return _pageStore.Pages;
            }
        }

        public IQueryable<ApplicationPage> AdminPages
        {
            get
            {
                return _pageStore.Pages.Include(x => x.ChildernPages).Include(x => x.ParentPages).Where(x => x.ParentPages.Any() == false);;
            }
        }

        public IQueryable<ApplicationPage> GetAnonymousPages()
        {
            return _pageStore.Pages.Include(x => x.ChildernPages).Include(x => x.ParentPages).Where(x => x.ParentPages.Any() == false && x.ApplicationRoles.Any(a=>a.Name == "Anonymous"));
        }

        //public IQueryable<ApplicationPage> Pages => _pageStore.Pages;

        //public IQueryable<ApplicationPage> AdminPages => _pageStore.Pages.Include(x => x.ChildernPages).Include(x => x.ParentPages).Where(x => x.ParentPages.Any() == false);

        public KeyValuePair<int, List<ApplicationPage>> SearchAllPaging(string keyword, int pageSize = 10, int pageNumber = 1)
        {
            int start = (pageNumber - 1) * pageSize;
            if (start < 0) start = 0;

            var pages = Pages;

            if (string.IsNullOrEmpty(keyword) == false)
            {
                keyword = keyword.ToLower();
                pages = pages.Where(p => p.NameEn.ToLower().Contains(keyword)
                || p.NameAr.ToLower().Contains(keyword));
            }
            var filterdItems = pages.ToList();
            return new KeyValuePair<int, List<ApplicationPage>>(filterdItems.Count(), pageSize == 0 ? filterdItems : filterdItems.Skip(start).Take(pageSize).ToList());
        }


        public IdentityResult CreatePage(ApplicationPage page,
            long[] parentPages,
            long[] childernPages)
        {
            if (parentPages != null && parentPages.Any())
            {
                page.ParentPages = _db.ApplicationPages.Where(p => parentPages.Contains(p.ApplicationPageId)).ToList();
            }

            if (childernPages != null && childernPages.Any())
            {
                page.ChildernPages = _db.ApplicationPages.Where(p => childernPages.Contains(p.ApplicationPageId)).ToList();
            }

            _pageStore.Create(page);
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> CreatePageAsync(ApplicationPage page,
            long[] parentPages,
             long[] childernPages)
        {
            if (parentPages != null && parentPages.Any())
            {
                page.ParentPages = await _db.ApplicationPages.Where(p => parentPages.Contains(p.ApplicationPageId)).ToListAsync();
            }

            if (childernPages != null && childernPages.Any())
            {
                page.ChildernPages = await _db.ApplicationPages.Where(p => childernPages.Contains(p.ApplicationPageId)).ToListAsync();
            }
            await _pageStore.CreateAsync(page);

            return IdentityResult.Success;
        }




        public IdentityResult SetPageParentsChildern(
                    long pageId,
                    long[] childern)
        {
            var thisPage = this.GetPageWithRelations(pageId);
            thisPage.ChildernPages.Clear();
            _db.SaveChanges();

            if (childern != null && childern.Any())
            {
                thisPage.ChildernPages = _db.ApplicationPages.Where(p => childern.Contains(p.ApplicationPageId)).ToList();
            }

            _db.SaveChanges();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetPageParentsChildernAsync(
                    long pageId,
                    long[] childern)
        {
            var thisPage = await this.GetPageWithRelationsAsync(pageId);
            thisPage.ChildernPages.Clear();
            await _db.SaveChangesAsync();

            if (childern != null && childern.Any())
            {
                thisPage.ChildernPages = await _db.ApplicationPages.Where(p => childern.Contains(p.ApplicationPageId)).ToListAsync();
            }

            await _db.SaveChangesAsync();

            return IdentityResult.Success;
        }



        public IdentityResult DeletePage(long pageId)
        {
            var page = this.FindById(pageId);
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            // remove the pages from the role:
            //page.ApplicationRoles.Clear();

            if (page.ChildernPages.Any())
            {
                page.ChildernPages.Clear();
            }

            // Remove the page itself:
            _db.ApplicationPages.Remove(page);
            _db.SaveChanges();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeletePageAsync(long pageId)
        {
            var page = await this.FindByIdAsync(pageId);
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            // remove the pages from the role:
            //page.ApplicationRoles.Clear();
            if (page.ChildernPages.Any())
            {
                page.ChildernPages.Clear();
            }
            // Remove the page itself:
            _db.ApplicationPages.Remove(page);

            await _db.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public IdentityResult DeletePages(List<long> pagesIds)
        {
            try
            {
                if (pagesIds == null || pagesIds.Any() == false)
                {
                    throw new ArgumentNullException("pages");
                }

                foreach (ApplicationPage page in Pages.Include(p => p.ChildernPages)
                    .Include(p=>p.ParentPages)
                    .Where(p => pagesIds.Contains(p.ApplicationPageId)))
                {
                    if (page.ChildernPages.Any())
                    {
                        page.ChildernPages.Clear();
                    }
                    _db.ApplicationPages.Remove(page);
                }

                // Remove the page itself:
                _db.SaveChanges();
                return IdentityResult.Success;
            }
            catch (Exception exc)
            {
                return IdentityResult.Failed(exc?.InnerException?.Message);
            }
        }

        public async Task<IdentityResult> DeletePagesAsync(List<long> pagesIds)
        {
            if (pagesIds == null || pagesIds.Any() == false)
            {
                throw new ArgumentNullException("pages");
            }


            foreach (ApplicationPage page  in Pages.Include(p => p.ChildernPages)
                .Include(p => p.ParentPages)
                .Where(p => pagesIds.Contains(p.ApplicationPageId)))
            {
                if (page.ChildernPages.Any())
                {
                    page.ChildernPages.Clear();
                }
                _db.ApplicationPages.Remove(page);
            }

            await _db.SaveChangesAsync();
            return IdentityResult.Success;
        }


        public IdentityResult UpdatePage(ApplicationPage page)
        {
            _pageStore.Update(page);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdatePageAsync(ApplicationPage page)
        {
            await _pageStore.UpdateAsync(page);

            return IdentityResult.Success;
        }



        public IdentityResult Activate(long id)
        {
            var currentPage = FindById(id);
            currentPage.Active = !currentPage.Active;
            return UpdatePage(currentPage);
        }

        public async Task<IdentityResult> ActivateAsync(long id)
        {
            var currentPage = await FindByIdAsync(id);
            currentPage.Active = !currentPage.Active;
            return await UpdatePageAsync(currentPage);
        }


        public ApplicationPage FindById(long id)
        {
            return _pageStore.FindById(id);
        }

        public async Task<ApplicationPage> FindByIdAsync(long id)
        {
            return await _pageStore.FindByIdAsync(id);
        }


        public ApplicationPage GetPageWithRelations(long pageId)
        {
            return _db.ApplicationPages
                .Include(p => p.ChildernPages)
                .Include(p => p.ParentPages)
                .FirstOrDefault(p => p.ApplicationPageId == pageId);
        }
        public async Task<ApplicationPage> GetPageWithRelationsAsync(long pageId)
        {
            return await _db.ApplicationPages
                .Include(p => p.ChildernPages)
                .Include(p => p.ParentPages)
                .FirstOrDefaultAsync(p => p.ApplicationPageId == pageId);
        }

        public ApplicationPage FindByName(string name)
        {
            return _pageStore.FindByName(name);
        }

        public async Task<ApplicationPage> FindByNameAsync(string name)
        {
            return await _pageStore.FindByNameAsync(name);
        }

        public bool UpdateRange(List<ApplicationPage> pages)
        {
            return _pageStore.CreateBulk(pages);
        }

        public async Task<bool> UpdateRangeAsync(List<ApplicationPage> pages)
        {
            return await _pageStore.CreateBulkAsync(pages);
        }

        public List<string> GetUserRolesIds(string userId)
        {
            return _db.Roles.Where(r => r.Users.Any(u => u.UserId == userId)).ToList().Select(r => r.Id).ToList();
        }


        public async Task<List<string>> GetUserRolesIdsAsync(string userId)
        {
            var roles = await _db.Roles.Where(r => r.Users.Any(u => u.UserId == userId)).ToListAsync();
            return roles.Select(r => r.Id).ToList();
        }


        #region Roles

        public IdentityResult SetRolePages(string roleId, params long[] pageIds)
        {
            // Clear all the Pages associated with this role:
            var thisRole = _roleManager.FindById(roleId);
            thisRole.ApplicationPages.Clear();
            _db.SaveChanges();

            // Add the new Page passed in:
            var newPages = _pageStore.Pages.Where(r => pageIds.Any(n => n == r.ApplicationPageId));
            foreach (var page in newPages)
            {
                thisRole.ApplicationPages.Add(page);
            }
            _db.SaveChanges();

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetRolePagesAsync(
            string roleId, params long[] pageIds)
        {
            // Clear all the Pages associated with this role:
            var thisRole = _roleManager.FindById(roleId);
            thisRole.ApplicationPages.Clear();
            await _db.SaveChangesAsync();

            // Add the new Page passed in:
            var newPages = _pageStore.Pages.Where(r => pageIds.Any(n => n == r.ApplicationPageId));
            foreach (var page in newPages)
            {
                thisRole.ApplicationPages.Add(page);
            }
            await _db.SaveChangesAsync();


            return IdentityResult.Success;
        }


        public IdentityResult ClearRolePages(string roleId)
        {
            return this.SetRolePages(roleId, new long() { });
        }

        public async Task<IdentityResult> ClearRolePagesAsync(string roleId)
        {
            return await this.SetRolePagesAsync(roleId, new long[] { });
        }


        public IEnumerable<ApplicationPage> GetRolePages(string roleId)
        {
            var rolePages = (from g in this.Pages
                             .Include(x => x.ChildernPages)
                             .Include(x => x.ParentPages)
                             where g.ApplicationRoles
                               .Any(u => u.Id == roleId)
                             select g).ToList();
            return rolePages;
        }

        public async Task<IEnumerable<ApplicationPage>> GetRolePagesAsync(string roleId)
        {
            var rolePages = (from g in this.Pages.Include(x => x.ChildernPages)
                             where g.ApplicationRoles
                               .Any(u => u.Id == roleId)
                             select g).ToListAsync();
            return await rolePages;
        }



        public List<ApplicationRole> GetPageRoles(long pageId)
        {
            var page = this.FindById(pageId);
            var roles = new List<ApplicationRole>();
            foreach (var role in page.ApplicationRoles)
            {
                var currentRole = _db.Roles.FirstOrDefault(u => u.Id == role.Id);

                if (currentRole != null && roles.FirstOrDefault(r => r.Id == currentRole.Id) == null)
                    roles.Add(currentRole);
            }
            return roles;
        }


        public async Task<IEnumerable<ApplicationPage>> GetRolesPagesAsync(List<string> rolesIds)
        {
            List<ApplicationPage> pageList = new List<ApplicationPage>();

            foreach (var role in rolesIds)
            {
                var pages = await GetRolePagesAsync(role);
                pageList.AddRange(pages);
            }

            return pageList;
        }



        public List<ApplicationPage> GetRolesPages(List<string> rolesIds)
        {
            List<ApplicationPage> pageList = new List<ApplicationPage>();

            foreach (var role in rolesIds)
            {
                var pages = GetRolePages(role);
                pageList.AddRange(pages);
            }
            return pageList;
        }

        public async Task<IEnumerable<ApplicationRole>> GetPageRolesAsync(long pageId)
        {
            var page = await this.FindByIdAsync(pageId);
            var roles = new List<ApplicationRole>();
            foreach (var role in page.ApplicationRoles)
            {
                var currentRole = await _db.Roles.FirstOrDefaultAsync(u => u.Id == role.Id);

                if (currentRole != null && roles.FirstOrDefault(r => r.Id == currentRole.Id) == null)
                    roles.Add(currentRole);
            }
            return roles;
        }

        public async Task<IEnumerable<ApplicationPage>> GetUserPagesAsync(string userId)
        {
            var currentUserRolesIds = await GetUserRolesIdsAsync(userId);
            return await GetRolesPagesAsync(currentUserRolesIds);
        }

        public IEnumerable<ApplicationPage> GetUserPages(string userId)
        {
            var currentUserRolesIds = GetUserRolesIds(userId);
            var pages = GetRolesPages(currentUserRolesIds);

            return pages.Where(p => p.Active).Where(x => x.ParentPages.Any() == false).ToList();
        }

        public bool IsRoleHasPage(string roleId, long pageId)
        {
            var grp = _db.ApplicationPages.FirstOrDefault(p => p.ApplicationPageId == pageId);
            return grp != null && grp.ApplicationRoles.Any();
        }

        public async Task<bool> IsRoleHasPageAsync(string roleId, long pageId)
        {
            var grp = await _db.ApplicationPages.FirstOrDefaultAsync(p => p.ApplicationPageId == pageId);
            return grp != null && grp.ApplicationRoles.Any();
        }

        #endregion

        /// <summary>
        /// Returns this class error message
        /// Call this methode in this class once any other method in this class failed to work
        /// </summary>
        /// <returns>Return error message string</returns>
        public string GetError()
        {
            return _errorMessage;
        }
    }
}
