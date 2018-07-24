using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LaborServices.Entity.Identity;
using LaborServices.Managers.Identity;
using LaborServices.Model.Identity;
using LaborServices.Web.Areas.Admin.Models;
using LaborServices.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using LaborServices.Web.Helpers;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class RolesAdminController : BaseController
    {
        private const int PageSize = 5;

        #region constructors

        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager,
            ApplicationPageManager pageManager,
            ApplicationGroupManager groupManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            PageManager = pageManager;
            GroupManager = groupManager;
        }
        #endregion

        #region managers
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private ApplicationPageManager _pageManager;
        public ApplicationPageManager PageManager
        {
            get
            {
                return _pageManager ?? new ApplicationPageManager(HttpContext.Request.GetOwinContext());
            }
            private set
            {
                _pageManager = value;
            }
        }

        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager(HttpContext.Request.GetOwinContext());
            }
            private set
            {
                _groupManager = value;
            }
        }
        #endregion

        //
        // GET: /Roles/
        [SetPermissions(nameAr: "الصلاحيات", nameEn: "Roles", controller: "RolesAdmin", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        [SetPermissions(nameAr: "عرض كل الصلاحيات", nameEn: "List all roles", controller: "RolesAdmin", action: "List", area: "Admin", isBaseParent: false)]
        public ActionResult List(int? page, string keyword)
        {
            int pageNumber = page ?? 1;
            KeyValuePair<int, List<ApplicationRole>> data = RoleManager.SearchAllPaging(pageNumber: pageNumber, pageSize: PageSize, keyword: keyword);
            var pagedList = new StaticPagedList<ApplicationRole>(data.Value, pageNumber, PageSize, data.Key);

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = PageSize;

            return PartialView("_List", pagedList);
        }
        //
        // GET: /Roles/Details/5
        [SetPermissions(nameAr: "تفاصيل الصلاحية", nameEn: "Details of role", controller: "RolesAdmin", action: "Details", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);

            var roleGroups = await GroupManager.GetRoleGroupsAsync(id);


            var groupsListIds = new List<string>();
            foreach (var group in roleGroups)
            {
                groupsListIds.AddRange(group.ApplicationUsers.Select(x => x.ApplicationUserId));
            }
            groupsListIds = groupsListIds.Distinct().ToList();

            // Get the list of Users in this Role
            var userList = UserManager.Users.Where(u => groupsListIds.Contains(u.Id) == false).ToList();

            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in userList)
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();

            ViewBag.Groups = roleGroups;
            return View(role);
        }

        //
        // GET: /Roles/Create
        [SetPermissions(nameAr: "انشاء صلاحية جديدة", nameEn: "create new Role", controller: "RolesAdmin", action: "Create", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Create(string id = "")
        {
            RoleViewModel roleModel = new RoleViewModel();


            var allPages = await this.PageManager.Pages.ToListAsync();
            var rolePages = string.IsNullOrEmpty(id) ? null : await this.PageManager.GetRolePagesAsync(id);


            var pageList = (from p in allPages
                            group p by p.Controller into g
                            select new PageGroupsViewModel
                            {
                                GroupName = g.Key == null ? "Other" : g.Key,
                                GroupsCount = g.Count(),
                                PageList = g.ToList().Select(
                                     p => new SelectListItem()
                                     {
                                         Text = p.NameEn + "|" + p.NameAr,
                                         Value = p.ApplicationPageId.ToString(),
                                         Selected = rolePages != null && rolePages.Any(r => r.ApplicationPageId == p.ApplicationPageId)
                                     }).ToList()
                            }).ToList();

            roleModel.PageGroupsViewModels = pageList;
            return View(roleModel);
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel,
            params long[] selectedPages)
        {
            ViewBag.PageList = this.PageManager.Pages.ToList();

            if (!ModelState.IsValid) return View(roleViewModel);
            var role = new ApplicationRole(roleViewModel.Role.Name);
            var roleResult = await RoleManager.CreateAsync(role);

            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError("", roleResult.Errors.First());
                return View(roleViewModel);
            }

            if (selectedPages != null)
                await this.PageManager.SetRolePagesAsync(role.Id, selectedPages);

            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Edit/Admin
        [SetPermissions(nameAr: "تعديل صلاحية", nameEn: "Edit role", controller: "RolesAdmin", action: "Edit", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            var allPages = await this.PageManager.Pages.ToListAsync();
            var rolePages = await this.PageManager.GetRolePagesAsync(id);
            var rolePageList = rolePages as List<ApplicationPage> ?? rolePages.ToList();

            var roleModel = new RoleViewModel { Role = role };

            var pageList = (from p in allPages
                            group p by p.Controller into g
                            select new PageGroupsViewModel
                            {
                                GroupName = g.Key == null ? "Other" : g.Key,
                                GroupsCount = g.Count(),
                                PageList = g.ToList().Select(
                                    p => new SelectListItem()
                                    {
                                        Text = p.NameEn + "|" + p.NameAr,
                                        Value = p.ApplicationPageId.ToString(),
                                        Selected = rolePageList != null && rolePageList.Any(r => r.ApplicationPageId == p.ApplicationPageId)
                                    }).ToList()
                            }).ToList();

            roleModel.PageGroupsViewModels = pageList;
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            RoleViewModel roleModel,
            params long[] selectedPages)
        {
            if (!ModelState.IsValid) return View(roleModel);
            var role = await RoleManager.FindByIdAsync(roleModel.Role.Id);
            role.Name = roleModel.Role.Name;
            var success = await RoleManager.UpdateAsync(role);
            if (success != IdentityResult.Success) return RedirectToAction("Index");
            selectedPages = selectedPages ?? new long[] { };
            await this.PageManager.SetRolePagesAsync(role.Id, selectedPages);
            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Delete/5
        [SetPermissions(nameAr: "حذف صلاحية", nameEn: "Delete role", controller: "RolesAdmin", action: "Delete", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
