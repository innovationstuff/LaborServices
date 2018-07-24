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
using LaborServices.Utility;
using LaborServices.Web.Areas.Admin.Models;
using LaborServices.Web.Helpers;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace LaborServices.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class GroupsAdminController : BaseController
    {
        private const int PageSize = 5;
        #region constructors

        public GroupsAdminController() { }

        public GroupsAdminController(
            ApplicationGroupManager groupManager,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager)
        {
            GroupManager = groupManager;
            RoleManager = roleManager;
            UserManager = userManager;
        }
        #endregion

        #region Managers
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                    .Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        [SetPermissions(nameAr: "المجموعات", nameEn: "Groups", controller: "GroupsAdmin", action: "Index", area: "Admin", isBaseParent: false)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int? page, string keyword)
        {
            int pageNumber = page ?? 1;
            KeyValuePair<int, List<ApplicationGroup>> data = GroupManager.SearchAllPaging(pageNumber: pageNumber, pageSize: PageSize, keyword: keyword);
            var pagedList = new StaticPagedList<ApplicationGroup>(data.Value, pageNumber, PageSize, data.Key);

            ViewBag.Keyword = keyword;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = PageSize;

            return PartialView("_List", pagedList);
        }

        [SetPermissions(nameAr: "تفاصيل المجموعة", nameEn: "Group details", controller: "GroupsAdmin", action: "Details", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.Groups.FirstOrDefaultAsync(g => g.Id == id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            var groupRoles = this.GroupManager.GetGroupRoles(id);
            string[] RoleNames = groupRoles.Select(p => p.Name).ToArray();

            ViewBag.RolesList = RoleNames;
            ViewBag.RolesCount = RoleNames.Count();

            var users = this.GroupManager.GetGroupUsers(id);

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();

            return View(applicationgroup);
        }


        [SetPermissions(nameAr: "انشاء مجموعة جديدة", nameEn: "Create new group", controller: "GroupsAdmin", action: "Create", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Create()
        {
            var allRoles = await this.RoleManager.Roles.ToListAsync();
            var allUsers = await this.UserManager.Users.Where(x => string.Equals(x.UserName, AppConstants.DefaultUserName) == false).ToListAsync();

            var model = new GroupViewModel();

            foreach (var role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                };
                model.RolesList.Add(listItem);
            }

            foreach (var user in allUsers)
            {
                var listItem = new SelectListItem()
                {
                    Text = user.UserName,
                    Value = user.Id,
                };
                model.UsersList.Add(listItem);
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GroupViewModel applicationgroup, string[] selectedRoles, string[] selectedUsers)
        {
            if (ModelState.IsValid)
            {
                // Create the new Group:

                var result = await this.GroupManager.CreateGroupAsync(applicationgroup.Group);
                if (result.Succeeded)
                {
                    // Add the roles selected:
                    if (selectedRoles != null)
                        await this.GroupManager.SetGroupRolesAsync(applicationgroup.Group.Id, selectedRoles);

                    if (selectedUsers != null)
                        await this.GroupManager.SetUsersGroupAsync(applicationgroup.Group.Id, selectedUsers);
                }
                return RedirectToAction("Index");
            }

            // Otherwise, start over:
            return View(applicationgroup);
        }


        [SetPermissions(nameAr: "تعديل مجموعة ", nameEn: "Edit group", controller: "GroupsAdmin", action: "Edit", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }

            // Get a list, not a DbSet or queryable:
            var allRoles = await this.RoleManager.Roles.ToListAsync();
            var groupRoles = await this.GroupManager.GetGroupRolesAsync(id);
            var groupRolesList = groupRoles as IList<ApplicationRole> ?? groupRoles.ToList();


            var allUsers = await this.UserManager.Users.Where(x => string.Equals(x.UserName, AppConstants.DefaultUserName) == false).ToListAsync();
            var groupUsers = await this.GroupManager.GetGroupUsersAsync(id);
            var groupUsersList = groupUsers as IList<ApplicationUser> ?? groupUsers.ToList();


            var model = new GroupViewModel {Group = applicationgroup};

            // load the roles/Roles for selection in the form:
            foreach (var role in allRoles)
            {
                var listItem = new SelectListItem()
                {
                    Text = role.Name,
                    Value = role.Id,
                    Selected = groupRolesList.Any(g => g.Id == role.Id)
                };
                model.RolesList.Add(listItem);
            }

            foreach (var user in allUsers)
            {
                var listItem = new SelectListItem()
                {
                    Text = user.UserName,
                    Value = user.Id,
                    Selected = groupUsersList.Any(g => g.Id == user.Id)
                };
                model.UsersList.Add(listItem);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GroupViewModel model, string[] selectedRoles, string[] selectedUsers)
        {
            var group = await this.GroupManager.FindByIdAsync(model.Group.Id);
            if (group == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                group.Name = model.Group.Name;
                group.Description = model.Group.Description;
                await this.GroupManager.UpdateGroupAsync(group);

                selectedRoles = selectedRoles ?? new string[] { };
                selectedUsers = selectedUsers ?? new string[] { };

                await this.GroupManager.SetGroupRolesAsync(group.Id, selectedRoles);
                await this.GroupManager.SetUsersGroupAsync(group.Id, selectedUsers);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [SetPermissions(nameAr: "تعديل مجموعة ", nameEn: "Edit group", controller: "GroupsAdmin", action: "Delete", area: "Admin", isBaseParent: false)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationGroup applicationgroup = await this.GroupManager.FindByIdAsync(id);
            if (applicationgroup == null)
            {
                return HttpNotFound();
            }
            return View(applicationgroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            await this.GroupManager.DeleteGroupAsync(id);
            return RedirectToAction("Index");
        }
    }
}